// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Cloning;
using System;
using System.Linq;

namespace AppBrix
{
    public static class CloningExtensions
    {
        /// <summary>
        /// Gets the registered <see cref="ICloner"/>.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The registered <see cref="ICloner"/>.</returns>
        public static ICloner GetCloner(this IApp app)
        {
            return app.Get<ICloner>();
        }
    }
}
