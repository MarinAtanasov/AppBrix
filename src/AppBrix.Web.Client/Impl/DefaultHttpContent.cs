// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web.Client.Impl
{
    internal sealed class DefaultHttpContent<T> : IHttpContent<T>
    {
        #region Construction
        public DefaultHttpContent(T body, IHttpContentHeaders headers)
        {
            this.Body = body;
            this.Headers = headers;
        }
        #endregion

        #region Properties
        public T Body { get; }

        public IHttpContentHeaders Headers { get; }
        #endregion
    }
}
