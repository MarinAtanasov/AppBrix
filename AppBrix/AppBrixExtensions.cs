// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix
{
    public static class AppBrixExtensions
    {
        /// <summary>
        /// Unloads and reloads the application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Restart(this IApp app)
        {
            app.Stop();
            app.Start();
        }

        /// <summary>
        /// Uninitializes and reinitializes the application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Reinitialize(this IApp app)
        {
            app.Uninitialize();
            app.Initialize();
        }

        /// <summary>
        /// Shorthand for getting the config from the currently defined <see cref="IConfigManager"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config.</typeparam>
        /// <param name="app">The current application.</param>
        /// <returns>The config.</returns>
        public static T GetConfig<T>(this IApp app) where T : class, IConfig
        {
            return app.ConfigManager.GetConfig<T>();
        }
    }
}
