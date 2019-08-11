// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Delayed;
using System;
using System.Linq;

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
        public static IDelayedEventHub GetDelayedEventHub(this IApp app)
        {
            return (IDelayedEventHub)app.Get(typeof(IDelayedEventHub));
        }
    }
}
