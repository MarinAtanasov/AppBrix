// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Client;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for the <see cref="WebClientModule"/>.
    /// </summary>
    public static class WebModuleExtensions
    {
        /// <summary>
        /// Sets the HTTP method using the given <see cref="HttpMethod"/>.
        /// </summary>
        /// <param name="request">The current REST request.</param>
        /// <param name="method">The HTTP method.</param>
        /// <returns>The current REST request.</returns>
        public static IHttpRequest SetMethod(this IHttpRequest request, HttpMethod method) => request.SetMethod(method.ToString().ToUpperInvariant());
    }
}
