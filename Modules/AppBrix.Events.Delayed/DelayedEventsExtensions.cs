// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Events.Delayed;
using AppBrix.Events.Delayed.Configuration;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix delayed events.
    /// </summary>
    public static class DelayedEventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded delayed event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static IDelayedEventHub GetDelayedEventHub(this IApp app) => (IDelayedEventHub)app.Get(typeof(IDelayedEventHub));

        /// <summary>
        /// Gets the <see cref="DelayedEventsConfig"/> from <see cref="IConfigService"/>.
        /// </summary>
        /// <param name="service">The configuration service.</param>
        /// <returns>The <see cref="DelayedEventsConfig"/>.</returns>
        public static DelayedEventsConfig GetDelayedEventsConfig(this IConfigService service) => (DelayedEventsConfig)service.Get(typeof(DelayedEventsConfig));
    }
}
