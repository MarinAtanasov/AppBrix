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
        public DefaultHttpContent(T data, IHttpContentHeaders headers)
        {
            this.Data = data;
            this.Headers = headers;
        }
        #endregion

        #region Properties
        public T Data { get; }

        public IHttpContentHeaders Headers { get; }
        #endregion
    }
}
