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
        /// Creates a default application with a specified path to the configuraiton.
        /// </summary>
        /// <param name="path">Path to the application configuration.</param>
        /// <returns>The created app.</returns>
        public static IApp Create(IConfigManager configManager)
        {
            return new DefaultApp(configManager);
        }
        #endregion
    }
}
