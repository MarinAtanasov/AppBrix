// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web.Impl
{
    internal sealed class DefaultRestResponse<T> : IRestResponse<T>
    {
        #region Construction
        public DefaultRestResponse(IRestMessageHeaders headers, IRestContent<T> content, int statusCode, string reasonPhrase, Version version)
        {
            this.Headers = headers;
            this.Content = content;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
            this.Version = version;
        }
        #endregion

        #region Properties
        public IRestMessageHeaders Headers { get; private set; }

        public IRestContent<T> Content { get; private set; }

        public int StatusCode { get; private set; }

        public string ReasonPhrase { get; private set; }

        public Version Version { get; private set; }
        #endregion
    }
}
