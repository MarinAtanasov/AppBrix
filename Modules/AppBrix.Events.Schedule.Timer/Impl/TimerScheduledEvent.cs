// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Schedule.Timer.Impl
{
    internal sealed class TimerScheduledEvent<T> : IScheduledEvent<T> where T : IEvent
    {
        #region Construction
        public TimerScheduledEvent(T args, DateTime initial, TimeSpan period)
        {
            this.Event = args;
            this.time = initial;
            this.period = period;
        }
        #endregion

        #region Properties
        public T Event { get; }
        #endregion

        #region Public and overriden methods
        public DateTime GetNextOccurrence(DateTime now)
        {
            if (this.period > TimeSpan.Zero)
            {
                while (this.time < now)
                {
                    this.time = this.time.Add(this.period);
                }
            }

            return this.time;
        }
        #endregion

        #region Private fields and constants
        private DateTime time;
        private readonly TimeSpan period;
        #endregion
    }
}
