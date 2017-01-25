// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using Microsoft.EntityFrameworkCore;
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
        /// Gets the application's currently registered <see cref="IDbContextService"/>
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The registered databse context service.</returns>
        public static IDbContextService GetDbContextService(this IApp app)
        {
            return (IDbContextService)app.Get(typeof(IDbContextService));
        }

        /// <summary>
        /// Gets an instance of a <see cref="DbContext"/> of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the context.</typeparam>
        /// <returns>A databse context of the provided type.</returns>
        public static T Get<T>(this IDbContextService service) where T : DbContext
        {
            return (T)service.Get(typeof(T));
        }

        /// <summary>
        /// Gets the currently registered <see cref="IDbContextConfigurer"/>.
        /// </summary>
        /// <param name="app">The current <see cref="IApp"/>.</param>
        /// <returns>The registered context configurer.</returns>
        internal static IDbContextConfigurer GetDbContextConfigurer(this IApp app)
        {
            return (IDbContextConfigurer)app.Get(typeof(IDbContextConfigurer));
        }
    }
}
