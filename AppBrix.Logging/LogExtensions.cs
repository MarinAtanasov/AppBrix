// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Logging;
using System;
using System.Linq;

namespace AppBrix
{
    public static class LogExtensions
    {
        /// <summary>
        /// Gets the registered log hub.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The log hub.</returns>
        public static ILogHub GetLog(this IApp app)
        {
            return app.Get<ILogHub>();
        }
    }
}
