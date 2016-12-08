// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Extension methods for the <see cref="DataModule"/>.
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Gets the application's currently registered <see cref="IContextLoader"/>
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The registered databse context loader.</returns>
        public static IContextLoader GetContextLoader(this IApp app)
        {
            return (IContextLoader)app.Get(typeof(IContextLoader));
        }

        internal static IDbContextConfigurer GetDbContextConfigurer(this IApp app)
        {
            return (IDbContextConfigurer)app.Get(typeof(IDbContextConfigurer));
        }
    }
}
