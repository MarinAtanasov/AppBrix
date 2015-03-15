// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AppBrix
{
    public static class ConfigExtensions
    {
        /// <summary>
        /// Shortcut for getting the configuration from the currently loaded IConfig.
        /// </summary>
        /// <typeparam name="T">The type of the configuration section.</typeparam>
        /// <param name="app">The application.</param>
        /// <returns>The application section.</returns>
        public static T GetConfig<T>(this IApp app) where T : ConfigurationSection
        {
            return app.Get<IConfig>().GetSection<T>();
        }
    }
}
