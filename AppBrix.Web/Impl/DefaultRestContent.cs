// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Web.Impl
{
    internal sealed class DefaultRestContent<T> : IRestContent<T>
    {
        #region Construction
        public DefaultRestContent(T content, IRestContentHeaders headers)
        {
            this.Content = content;
            this.Headers = headers;
        }
        #endregion

        #region Properties
        public T Content { get; private set; }

        public IRestContentHeaders Headers { get; private set; }
        #endregion
    }
}
