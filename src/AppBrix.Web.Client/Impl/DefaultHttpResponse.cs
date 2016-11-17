// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web.Client.Impl
{
    internal sealed class DefaultHttpResponse<T> : IHttpResponse<T>
    {
        #region Construction
        public DefaultHttpResponse(IHttpMessageHeaders headers, T content, int statusCode, string reasonPhrase, Version version)
        {
            this.Headers = headers;
            this.Content = content;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
            this.Version = version;
        }
        #endregion

        #region Properties
        public IHttpMessageHeaders Headers { get; }

        public T Content { get; }

        public int StatusCode { get; }

        public string ReasonPhrase { get; }

        public Version Version { get; }
        #endregion
    }
}
