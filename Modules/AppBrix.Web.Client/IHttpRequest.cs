// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Web.Client
{
    /// <summary>
    /// An object used for making an HTTP request.
    /// </summary>
    public interface IHttpRequest
    {
        /// <summary>
        /// Executes the HTTP request without returning the response content.
        /// </summary>
        /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The HTTP response.</returns>
        Task<IHttpResponse> Send(CancellationToken token = default);

        /// <summary>
        /// Executes the HTTP request.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The HTTP response.</returns>
        Task<IHttpResponse<T>> Send<T>(CancellationToken token = default);

        /// <summary>
        /// Sets an HTTP message header.
        /// </summary>
        /// <param name="header">The header's key.</param>
        /// <param name="values">The header's values.</param>
        /// <returns></returns>
        IHttpRequest SetHeader(string header, params string[] values);

        /// <summary>
        /// Sets the name of the <see cref="HttpClient"/> to be used when sending the request.
        /// Requires a registered <see cref="IHttpClientFactory"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="HttpClient"/>.</param>
        /// <returns></returns>
        IHttpRequest SetClientName(string name);

        /// <summary>
        /// Sets the HTTP request message content.
        /// </summary>
        /// <param name="content">The content to be added to the request.</param>
        /// <returns></returns>
        IHttpRequest SetContent(object content);

        /// <summary>
        /// Sets the target URL of the HTTP request.
        /// </summary>
        /// <param name="url">The URL to the REST service.</param>
        /// <returns></returns>
        IHttpRequest SetUrl(string url);

        /// <summary>
        /// Sets the HTTP method using the given <see cref="HttpMethod"/>.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <returns>The current REST request.</returns>
        public IHttpRequest SetMethod(HttpMethod method) => this.SetMethod(method.ToString().ToUpperInvariant());

        /// <summary>
        /// Sets the HTTP method which will be used when making the request.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <returns></returns>
        IHttpRequest SetMethod(string method);

        /// <summary>
        /// Return the HTTP version to be used.
        /// </summary>
        /// <param name="version">The HTTP version.</param>
        /// <returns></returns>
        IHttpRequest SetVersion(Version version);
    }
}
