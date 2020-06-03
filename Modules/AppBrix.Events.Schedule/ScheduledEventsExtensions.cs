// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Events.Schedule;
using AppBrix.Events.Schedule.Configuration;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix scheduled events.
    /// </summary>
    public static class ScheduledEventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded scheduled event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static IScheduledEventHub GetScheduledEventHub(this IApp app) => (IScheduledEventHub)app.Get(typeof(IScheduledEventHub));

        /// <summary>
        /// Gets the <see cref="ScheduledEventsConfig"/> from <see cref="IConfigService"/>.
        /// </summary>
        /// <param name="service">The configuration service.</param>
        /// <returns>The <see cref="ScheduledEventsConfig"/>.</returns>
        public static ScheduledEventsConfig GetScheduledEventsConfig(this IConfigService service) => (ScheduledEventsConfig)service.Get(typeof(ScheduledEventsConfig));
    }
}
