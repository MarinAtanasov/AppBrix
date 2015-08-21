// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web
{
    /// <summary>
    /// An object used for making an HTTP REST service call.
    /// </summary>
    public interface IRestCall
    {
        /// <summary>
        /// Executes the REST call.
        /// </summary>
        /// <typeparam name="T">The type of the response object to be returned.</typeparam>
        /// <returns>The REST response.</returns>
        IRestResponse<T> MakeCall<T>();

        /// <summary>
        /// Sets an HTTP message header.
        /// </summary>
        /// <param name="header">The header's key.</param>
        /// <param name="values">The header's values.</param>
        /// <returns></returns>
        IRestCall SetHeader(string header, params string[] values);

        /// <summary>
        /// Sets the HTTP message content.
        /// </summary>
        /// <typeparam name="T">The type of the content.</typeparam>
        /// <param name="content">The content to be added to the request.</param>
        /// <returns></returns>
        IRestCall SetContent<T>(T content);

        /// <summary>
        /// Sets an HTTP content header.
        /// </summary>
        /// <param name="header">The header's key.</param>
        /// <param name="values">The header's values.</param>
        /// <returns></returns>
        IRestCall SetContentHeader(string header, params string[] values);

        /// <summary>
        /// Sets the URL to the REST service.
        /// </summary>
        /// <param name="url">The URL to the REST service.</param>
        /// <returns></returns>
        IRestCall SetUrl(string url);

        IRestCall SetMethod(string method);

        IRestCall SetTimeout(TimeSpan timeout);

        IRestCall SetVersion(Version version);
    }
}
