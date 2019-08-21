// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Async;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix asynchronous events.
    /// </summary>
    public static class AsyncEventsExtensions
    {
        /// <summary>
        /// Gets the currently loaded asynchronous event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static IAsyncEventHub GetAsyncEventHub(this IApp app) => (IAsyncEventHub)app.Get(typeof(IAsyncEventHub));
    }
}
