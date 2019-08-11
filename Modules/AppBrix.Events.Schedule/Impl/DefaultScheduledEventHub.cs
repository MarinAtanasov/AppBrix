// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Schedule.Configuration;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AppBrix.Events.Schedule.Impl
{
    internal sealed class DefaultScheduledEventHub : IScheduledEventHub, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
            this.config = this.app.GetConfig<ScheduledEventsConfig>();
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
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var now = this.app.GetTime();
            var item = new PriorityQueueItem<T>(this.app, args);
            item.MoveToNextOccurrence(now);
            if (now < item.Occurrence)
            {
                lock (this.queue)
                {
                    this.queue.Push(item);
                }
            }
        }

        public void Unschedule<T>(IScheduledEvent<T> args) where T : IEvent
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            lock (this.queue)
            {
                this.queue.Remove(args);
            }
        }
        #endregion

        #region Private methods
        private void ExecuteReadyEvents(object unused = null)
        {
            List<PriorityQueueItem> toExecute = null;
            lock (this.queue)
            {
                if (this.executionTimer == null)
                    return; // Unintialized

                var now = this.app.GetTime();
                for (var args = this.queue.Peek(); args != null && args.Occurrence <= now; args = this.queue.Peek())
                {
                    if (toExecute == null)
                        toExecute = new List<PriorityQueueItem>();

                    toExecute.Add(args);
                    args.MoveToNextOccurrence(now);
                    if (now < args.Occurrence)
                    {
                        this.queue.ReprioritizeHead();
                    }
                    else
                    {
                        this.queue.Pop();
                    }
                }

                this.executionTimer.Change(this.config.ExecutionCheck, Timeout.InfiniteTimeSpan);
            }
            toExecute?.ForEach(x => x.Execute());
        }
        #endregion

        #region Private fields and constants
        private readonly PriorityQueue queue = new PriorityQueue();
        private IApp app;
        private ScheduledEventsConfig config;
        private Timer executionTimer;
        #endregion
    }
}
