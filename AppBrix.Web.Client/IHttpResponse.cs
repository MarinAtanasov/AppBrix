// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web.Client
{
    /// <summary>
    /// HTTP response object which is returned by <see cref="IHttpCall"/>.
    /// </summary>
    /// <typeparam name="T">The type of the response content.</typeparam>
    public interface IHttpResponse<T>
    {
        /// <summary>
        /// Gets the HTTP response headers.
        /// </summary>
        IHttpMessageHeaders Headers { get; }

        /// <summary>
        /// Gets the HTTP response content.
        /// </summary>
        IHttpContent<T> Content { get; }

        /// <summary>
        /// Gets the HTTP response status code.
        /// </summary>
        int StatusCode { get; }

        /// <summary>
        /// Gets the HTTP response reason phrase.
        /// </summary>
        string ReasonPhrase { get; }

        /// <summary>
        /// Gets the HTTP version.
        /// </summary>
        Version Version { get; }
    }
}
