// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web
{
    /// <summary>
    /// HTTP message content.
    /// </summary>
    /// <typeparam name="T">The type of the content object.</typeparam>
    public interface IRestContent<T>
    {
        /// <summary>
        /// Gets the content headers.
        /// </summary>
        IRestContentHeaders Headers { get; }

        /// <summary>
        /// Gets the message content.
        /// </summary>
        T Content { get; }
    }
}
