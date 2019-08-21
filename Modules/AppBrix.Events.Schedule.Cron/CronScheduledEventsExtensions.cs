// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Schedule.Cron;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix cron scheduled events.
    /// </summary>
    public static class CronScheduledEventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded cron scheduled event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static ICronScheduledEventHub GetCronScheduledEventHub(this IApp app) => (ICronScheduledEventHub)app.Get(typeof(ICronScheduledEventHub));
    }
}
