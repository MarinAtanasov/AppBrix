// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Static class used for loading of a default app.
    /// </summary>
    public static class App
    {
        #region Public methods
        /// <summary>
        /// Creates a default application with configuraiton from the entry assembly.
        /// </summary>
        /// <returns>The created app.</returns>
        public static IApp Create()
        {
            return App.Create(new DefaultAppConfig());
        }

        /// <summary>
        /// Creates a default application with a specified path to the configuraiton.
        /// </summary>
        /// <param name="path">Path to the application configuration.</param>
        /// <returns>The created app.</returns>
        public static IApp Create(string path)
        {
            return App.Create(new DefaultAppConfig(path));
        }

        /// <summary>
        /// Creates a default application with a specified app configuraiton.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        /// <returns>The created app.</returns>
        public static IApp Create(IAppConfig config)
        {
            return new DefaultApp(config);
        }
        #endregion
    }
}
