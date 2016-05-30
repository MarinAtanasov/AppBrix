// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events.Async;
using System;
using System.Linq;

namespace AppBrix
{
    public static class AsyncEventExtensions
    {
        /// <summary>
        /// Gets the currently loaded asynchronous event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static IAsyncEventHub GetAsyncEventHub(this IApp app)
        {
            return app.Get<IAsyncEventHub>();
        }
    }
}
