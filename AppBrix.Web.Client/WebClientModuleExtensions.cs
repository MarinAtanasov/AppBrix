// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Client;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for the <see cref="WebClientModule"/>.
    /// </summary>
    public static class WebModuleExtensions
    {
        /// <summary>
        /// Sets the HTTP method using the given <see cref="HttpCallMethod"/>.
        /// </summary>
        /// <param name="call">The current REST call.</param>
        /// <param name="method">The HTTP method.</param>
        /// <returns>The current REST call.</returns>
        public static IHttpCall SetMethod(this IHttpCall call, HttpCallMethod method)
        {
            return call.SetMethod(method.ToString());
        }
    }
}
