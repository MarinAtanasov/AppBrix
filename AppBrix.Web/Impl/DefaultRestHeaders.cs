// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Web.Impl
{
    internal sealed class DefaultRestHeaders : Dictionary<string, string[]>, IRestMessageHeaders, IRestContentHeaders
    {
        public DefaultRestHeaders()
        {
        }

        public DefaultRestHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            foreach (var header in headers)
            {
                this[header.Key] = header.Value.ToArray();
            }
        }
    }
}
