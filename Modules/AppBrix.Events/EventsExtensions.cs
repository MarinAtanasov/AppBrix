// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix events.
    /// </summary>
    public static class EventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static IEventHub GetEventHub(this IApp app) => (IEventHub)app.Get(typeof(IEventHub));
    }
}
