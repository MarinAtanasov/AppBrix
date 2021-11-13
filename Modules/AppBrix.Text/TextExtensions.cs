// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Text;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for the <see cref="TextModule"/>.
    /// </summary>
    public static class TextExtensions
    {
        /// <summary>
        /// Gets the application's currently registered <see cref="IStringDistanceService"/>
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The registered string distance service.</returns>
        public static IStringDistanceService GetStringDistanceService(this IApp app) => (IStringDistanceService)app.Get(typeof(IStringDistanceService));
    }
}
