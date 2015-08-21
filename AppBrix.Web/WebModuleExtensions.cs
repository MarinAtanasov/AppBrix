// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for the <see cref="WebModule"/>.
    /// </summary>
    public static class WebModuleExtensions
    {
        /// <summary>
        /// Sets the HTTP method using the given <see cref="RestCallMethod"/>.
        /// </summary>
        /// <param name="call">The current REST call.</param>
        /// <param name="method">The HTTP method.</param>
        /// <returns>The current REST call.</returns>
        public static IRestCall SetMethod(this IRestCall call, RestCallMethod method)
        {
            return call.SetMethod(method.ToString());
        }
    }
}
