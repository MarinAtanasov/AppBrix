// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Web.Client.Impl
{
    internal sealed class HttpResponse<T> : IHttpResponse<T>
    {
        #region Construction
        public HttpResponse(IHttpHeaders headers, T content, int statusCode, string reasonPhrase, Version version)
        {
            this.Headers = headers;
            this.Content = content;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
            this.Version = version;
        }
        #endregion

        #region Properties
        public IHttpHeaders Headers { get; }

        public T Content { get; }

        public int StatusCode { get; }

        public string ReasonPhrase { get; }

        public Version Version { get; }
        #endregion
    }
}
