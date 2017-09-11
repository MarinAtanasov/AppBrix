// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events;
using AppBrix.Events.Schedule;
using AppBrix.Events.Schedule.Timer;
using System;
using System.Linq;
using System.Threading;

namespace AppBrix
{
    public static class TimerScheduledEventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded cron scheduled event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static ITimerScheduledEventHub GetTimerScheduledEventHub(this IApp app)
        {
            return (ITimerScheduledEventHub)app.Get(typeof(ITimerScheduledEventHub));
        }

        /// <summary>
        /// Schedule an <see cref="IScheduledEvent"/> to be executed once.
        /// </summary>
        /// <param name="args">The event to be executed.</param>
        /// <param name="dueTime">The amount of time to delay before the event should be raised.</param>
        /// <returns>The scheduled event, containing the original event.</returns>
        public static IScheduledEvent<T> Schedule<T>(this ITimerScheduledEventHub eventHub, T args, TimeSpan dueTime)
            where T : IEvent
        {
            return eventHub.Schedule(args, dueTime, Timeout.InfiniteTimeSpan);
        }
    }
}
