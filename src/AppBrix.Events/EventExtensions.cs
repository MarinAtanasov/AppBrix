// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events;
using System;
using System.Linq;

namespace AppBrix
{
    public static class EventExtensions
    {
        /// <summary>
        /// Gets the currently loaded event hub.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The event hub.</returns>
        public static IEventHub GetEventHub(this IApp app)
        {
            return app.Get<IEventHub>();
        }
    }
}
