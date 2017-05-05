// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
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
        /// <returns>The HTTP response.</returns>
        Task<IHttpResponse> Send();

        /// <summary>
        /// Executes the HTTP request.
        /// Can return <see cref="string"/>, <see cref="System.IO.Stream"/> or <see cref="T:byte[]"/>.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <returns>The HTTP response.</returns>
        Task<IHttpResponse<T>> Send<T>();

        /// <summary>
        /// Sets an HTTP message header.
        /// </summary>
        /// <param name="header">The header's key.</param>
        /// <param name="values">The header's values.</param>
        /// <returns></returns>
        IHttpRequest SetHeader(string header, params string[] values);

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
        /// Sets the HTTP method which will be used when making the request.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <returns></returns>
        IHttpRequest SetMethod(string method);

        /// <summary>
        /// Sets the timeout for the request.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        IHttpRequest SetTimeout(TimeSpan timeout);
        
        /// <summary>
        /// Return the HTTP version to be used.
        /// </summary>
        /// <param name="version">The HTTP version.</param>
        /// <returns></returns>
        IHttpRequest SetVersion(Version version);
    }
}
