// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web.Client
{
    /// <summary>
    /// HTTP message content.
    /// </summary>
    /// <typeparam name="T">The type of the content object.</typeparam>
    public interface IHttpContent<out T>
    {
        /// <summary>
        /// Gets the message content.
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Gets the content headers.
        /// </summary>
        IHttpContentHeaders Headers { get; }
    }
}
