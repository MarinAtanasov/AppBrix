// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events.Schedule.Cron;
using System;
using System.Linq;

namespace AppBrix
{
    public static class CronScheduledEventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded cron scheduled event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static ICronScheduledEventHub GetCronScheduledEventHub(this IApp app)
        {
            return (ICronScheduledEventHub)app.Get(typeof(ICronScheduledEventHub));
        }
    }
}
