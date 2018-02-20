// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Schedule;
using System;
using System.Linq;

namespace AppBrix
{
    public static class ScheduledEventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded scheduled event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static IScheduledEventHub GetScheduledEventHub(this IApp app)
        {
            return (IScheduledEventHub)app.Get(typeof(IScheduledEventHub));
        }
    }
}
