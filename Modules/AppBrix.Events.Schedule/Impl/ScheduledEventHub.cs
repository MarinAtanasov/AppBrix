// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Configuration;
using AppBrix.Events.Schedule.Contracts;
using AppBrix.Events.Schedule.Services;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AppBrix.Events.Schedule.Impl;

internal sealed class ScheduledEventHub : IScheduledEventHub, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
        this.config = this.app.ConfigService.GetScheduledEventsConfig();
        var timeout = this.config.ExecutionCheck;
        lock (this.queue)
        {
            this.executionTimer = new Timer(this.ExecuteReadyEvents, null, timeout, Timeout.InfiniteTimeSpan);
        }
    }

    public void Uninitialize()
    {
        lock (this.queue)
        {
            this.executionTimer?.Dispose();
            this.executionTimer = null;
            this.queue.Clear();
        }

        this.config = null;
        this.app = null;
    }
    #endregion

    #region IScheduledEventHub implementation
    public void Schedule<T>(IScheduledEvent<T> args) where T : IEvent
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        var item = new PriorityQueueItem<T>(this.app, args);
        item.MoveToNextOccurrence(this.app.GetTime());

        lock (this.queue)
        {
            this.queue.Push(item);
        }
    }

    public void Unschedule<T>(IScheduledEvent<T> args) where T : IEvent
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        lock (this.queue)
        {
            this.queue.Remove(args);
        }
    }
    #endregion

    #region Private methods
    private void ExecuteReadyEvents(object? unused = null)
    {
        List<PriorityQueueItem>? toExecute = null;

        lock (this.queue)
        {
            if (this.executionTimer is null)
                return; // Unintialized

            var now = this.app.GetTime();
            for (var args = this.queue.Peek(); args is not null && args.Occurrence <= now; args = this.queue.Peek())
            {
                toExecute ??= new List<PriorityQueueItem>();
                toExecute.Add(args);
                args.MoveToNextOccurrence(now);
                if (now < args.Occurrence)
                    this.queue.ReprioritizeHead();
                else
                    this.queue.Pop();
            }

            this.executionTimer.Change(this.config.ExecutionCheck, Timeout.InfiniteTimeSpan);
        }

        toExecute?.ForEach(x => x.Execute());
    }
    #endregion

    #region Private fields and constants
    private readonly PriorityQueue queue = new PriorityQueue();
    #nullable disable
    private IApp app;
    private ScheduledEventsConfig config;
    private Timer executionTimer;
    #nullable restore
    #endregion
}
