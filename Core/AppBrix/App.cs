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
        /// Creates a default application with a specified configuration service.
        /// </summary>
        /// <param name="configService">The configuration service.</param>
        /// <returns>The created app.</returns>
        public static IApp Create(IConfigService configService)
        {
            return new DefaultApp(configService);
        }
        #endregion
    }
}
