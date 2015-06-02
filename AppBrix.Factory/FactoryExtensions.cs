// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Factory;
using System;
using System.Linq;

namespace AppBrix
{
    public static class EventExtensions
    {
        /// <summary>
        /// Gets the currently loaded factory.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The factory.</returns>
        public static IFactory GetFactory(this IApp app)
        {
            return app.Get<IFactory>();
        }
    }
}
