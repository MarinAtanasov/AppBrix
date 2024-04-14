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
using System.Threading.Tasks;

namespace AppBrix.Events.Schedule.Impl;

internal sealed class ScheduledEventHub : IScheduledEventHub, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
        this.config = this.app.ConfigService.GetScheduledEventsConfig();
        this.timer = new PeriodicTimer(this.config.ExecutionCheck);

        this.app.GetAsyncEventHub().Subscribe<PriorityQueueItem>(this.PriorityQueueItemRaised);
        this.cts = new CancellationTokenSource();
        _ = this.Run(this.cts.Token);
    }

    public void Uninitialize()
    {
        lock (this.queue)
        {
            this.timer.Dispose();
            this.cts?.Cancel();
            this.cts = null;
            this.executing.Clear();
            this.queue.Clear();
        }

        this.app.GetAsyncEventHub().Unsubscribe<PriorityQueueItem>(this.PriorityQueueItemRaised);
        this.config = null!;
        this.app = null!;
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
    private void PriorityQueueItemRaised(PriorityQueueItem args) => args.Execute();

    private async Task Run(CancellationToken token)
    {
        while (await this.timer.WaitForNextTickAsync(token).ConfigureAwait(false))
        {
            var now = this.app.GetTime();
            lock (this.queue)
            {
                token.ThrowIfCancellationRequested();  // Uninitialized
                for (var args = this.queue.Peek(); args is not null && args.Occurrence <= now; args = this.queue.Peek())
                {
                    this.executing.Add(args);
                    args.MoveToNextOccurrence(now);
                    if (now < args.Occurrence)
                        this.queue.ReprioritizeHead();
                    else
                        this.queue.Pop();
                }
            }

            if (this.executing.Count == 0)
                continue;

            for (var i = 0; i < this.executing.Count; i++)
            {
                this.app.GetAsyncEventHub().Raise(this.executing[i]);
            }

            this.executing.Clear();
        }
    }
    #endregion

    #region Private fields and constants
    private readonly PriorityQueue queue = new PriorityQueue();
    private readonly List<PriorityQueueItem> executing = new List<PriorityQueueItem>();
    private CancellationTokenSource? cts;
    private IApp app = null!;
    private ScheduledEventsConfig config = null!;
    private PeriodicTimer timer = null!;
    #endregion
}
