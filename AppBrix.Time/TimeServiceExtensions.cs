// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Time;
using System;
using System.Linq;

namespace AppBrix
{
    public static class TimeServiceExtensions
    {
        /// <summary>
        /// A shorthand for getting the current time
        /// from the registered time service.
        /// </summary>
        /// <param name="app">The currently running application.</param>
        /// <returns>The current DateTime.</returns>
        public static DateTime GetTime(this IApp app)
        {
            return app.Get<ITimeService>().GetTime();
        }
    }
}
