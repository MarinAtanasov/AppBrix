// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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
        public static IHttpRequest SetMethod(this IHttpRequest request, AppBrix.Web.Client.HttpMethod method)
        {
            return request.SetMethod(method.ToString().ToUpperInvariant());
        }

        internal static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
