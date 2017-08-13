// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events.Schedule.Config;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AppBrix.Events.Schedule.Impl
{
    internal class DefaultScheduledEventHub : IScheduledEventHub, IApplicationLifecycle
    {
        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
            this.queue.Initialize(context);
            var timeout = this.app.GetConfig<ScheduledEventsConfig>().ExecutionCheck;
            lock (this.queue)
            {
                this.expirationTimer = new Timer(this.ExecuteReadyEvents, null, timeout, TimeSpan.FromMilliseconds(-1));
            }
        }

        public void Uninitialize()
        {
            lock (this.queue)
            {
                this.expirationTimer?.Dispose();
                this.expirationTimer = null;
            }

            this.queue.Uninitialize();
            this.app = null;
        }

        public void Schedule<T>(IScheduledEvent<T> args) where T : IEvent
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            lock (this.queue)
            {
                var item = new PriorityQueueItem<T>(args, () => this.ExecuteEvent(args.Event));
                item.MoveToNextOccurrence(this.app.GetTime());
                this.queue.Push(item);
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
                if (this.expirationTimer == null)
                    return; // Unintialized

                var now = this.app.GetTime();

                while ((this.queue.Peek()?.Occurrence ?? now) < now)
                {
                    var args = this.queue.Pop();
                    args.MoveToNextOccurrence(now);
                    this.queue.Push(args);

                    if (toExecute == null)
                        toExecute = new List<PriorityQueueItem>();
                    toExecute.Add(args);
                }

                this.expirationTimer.Change(this.app.GetConfig<ScheduledEventsConfig>().ExecutionCheck, TimeSpan.FromMilliseconds(-1));
            }
            toExecute?.ForEach(x => x.Execute());
        }

        private void ExecuteEvent<T>(T args) where T : IEvent
        {
            this.app.GetEventHub().Raise(args);
        }
        #endregion

        #region Private fields and constants
        private readonly PriorityQueue queue = new PriorityQueue();
        private IApp app;
        private Timer expirationTimer;
        #endregion
    }
}
