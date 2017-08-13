// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Schedule.Impl
{
    internal abstract class PriorityQueueItem
    {
        #region Construction
        public PriorityQueueItem(object args, Action execute)
        {
            this.Event = args;
            this.Execute = execute;
        }
        #endregion

        #region Properties
        public object Event { get; }

        public Action Execute { get; }

        public DateTime Occurrence { get; protected set; }
        #endregion

        #region Public methods
        public abstract void MoveToNextOccurrence(DateTime now);
        #endregion
    }

    internal class PriorityQueueItem<T> : PriorityQueueItem where T : IEvent
    {
        #region Construction
        public PriorityQueueItem(IScheduledEvent<T> scheduledEvent, Action execute)
            : base(scheduledEvent, execute)
        {
            this.ScheduledEvent = scheduledEvent;
        }
        #endregion

        #region Properties
        public IScheduledEvent<T> ScheduledEvent { get; }
        #endregion

        #region Public methods
        public override void MoveToNextOccurrence(DateTime now)
        {
            this.Occurrence = this.ScheduledEvent.GetNextOccurrence(now);
        }
        #endregion
    }
}
