// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Events.Schedule
{
    /// <summary>
    /// Scheduler for events which decides when they need to be called.
    /// </summary>
    public interface IScheduledEventHub
    {
        /// <summary>
        /// Schedule an <see cref="IScheduledEvent{T}"/> to be executed.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="args">The scheduled event.</param>
        void Schedule<T>(IScheduledEvent<T> args) where T : IEvent;

        /// <summary>
        /// Unschedules an <see cref="IScheduledEvent{T}"/> to stop it from executing.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="args">The scheduled event.</param>
        void Unschedule<T>(IScheduledEvent<T> args) where T : IEvent;
    }
}
