// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Web.Client.Contracts;

/// <summary>
/// HTTP response object which is returned by <see cref="IHttpRequest"/>.
/// </summary>
public interface IHttpResponse
{
    /// <summary>
    /// Gets the HTTP response headers.
    /// </summary>
    IHttpHeaders Headers { get; }

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

/// <summary>
/// HTTP response object which is returned by <see cref="IHttpRequest"/>.
/// </summary>
/// <typeparam name="T">The type of the response content.</typeparam>
public interface IHttpResponse<out T> : IHttpResponse
{
    /// <summary>
    /// Gets the HTTP response content body.
    /// </summary>
    T Content { get; }
}
