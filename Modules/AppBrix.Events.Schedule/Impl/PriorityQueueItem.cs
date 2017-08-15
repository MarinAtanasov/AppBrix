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
        public PriorityQueueItem(object scheduledEvent, Action execute)
        {
            this.ScheduledEvent = scheduledEvent;
            this.Execute = execute;
        }
        #endregion

        #region Properties
        public object ScheduledEvent { get; }

        public Action Execute { get; }

        public DateTime Occurrence { get; protected set; }
        #endregion

        #region Public and overriden methods
        public abstract void MoveToNextOccurrence(DateTime now);
        #endregion
    }

    internal sealed class PriorityQueueItem<T> : PriorityQueueItem where T : IEvent
    {
        #region Construction
        public PriorityQueueItem(IScheduledEvent<T> scheduledEvent, Action execute)
            : base(scheduledEvent, execute)
        {
        }
        #endregion

        #region Public and overriden methods
        public override void MoveToNextOccurrence(DateTime now)
        {
            this.Occurrence = ((IScheduledEvent<T>)this.ScheduledEvent).GetNextOccurrence(now);
        }
        #endregion
    }
}
