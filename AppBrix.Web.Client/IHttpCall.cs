// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Web.Client
{
    /// <summary>
    /// An object used for making an HTTP REST service call.
    /// </summary>
    public interface IHttpCall
    {
        /// <summary>
        /// Executes the REST call.
        /// Can return <see cref="string"/>, <see cref="System.IO.Stream"/> or <see cref="byte[]"/>.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <returns>The REST response.</returns>
        Task<IHttpResponse<T>> MakeCall<T>();

        /// <summary>
        /// Sets an HTTP message header.
        /// </summary>
        /// <param name="header">The header's key.</param>
        /// <param name="values">The header's values.</param>
        /// <returns></returns>
        IHttpCall SetHeader(string header, params string[] values);

        /// <summary>
        /// Sets the HTTP message content.
        /// </summary>
        /// <typeparam name="T">The type of the content.</typeparam>
        /// <param name="content">The content to be added to the request.</param>
        /// <returns></returns>
        IHttpCall SetContent<T>(T content);

        /// <summary>
        /// Sets an HTTP content header.
        /// </summary>
        /// <param name="header">The header's key.</param>
        /// <param name="values">The header's values.</param>
        /// <returns></returns>
        IHttpCall SetContentHeader(string header, params string[] values);

        /// <summary>
        /// Sets the URL to the REST service.
        /// </summary>
        /// <param name="url">The URL to the REST service.</param>
        /// <returns></returns>
        IHttpCall SetUrl(string url);

        /// <summary>
        /// Sets the HTTP method which will be used when making the call.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <returns></returns>
        IHttpCall SetMethod(string method);

        /// <summary>
        /// Sets the timeout for the call.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        IHttpCall SetTimeout(TimeSpan timeout);
        
        /// <summary>
        /// Return the HTTP version to be used.
        /// </summary>
        /// <param name="version">The HTTP version.</param>
        /// <returns></returns>
        IHttpCall SetVersion(Version version);
    }
}
