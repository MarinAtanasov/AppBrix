// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Schedule.Cron
{
    /// <summary>
    /// Scheduler for cron based events which decides when they need to be called.
    /// </summary>
    public interface ICronScheduledEventHub
    {
        /// <summary>
        /// Schedule an <see cref="IScheduledEvent"/> to be executed.
        /// </summary>
        /// <param name="args">The event to be executed.</param>
        /// <param name="expresssion">The cron expression which decides when the event should be called.</param>
        /// <returns>The scheduled event, containing the original event.</returns>
        IScheduledEvent<T> Schedule<T>(T args, string expresssion) where T : IEvent;

        /// <summary>
        /// Unschedules an <see cref="IScheduledEvent"/> to stop it from executing.
        /// </summary>
        /// <param name="args">The scheduled event.</param>
        void Unschedule<T>(IScheduledEvent<T> args) where T : IEvent;
    }
}
