// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System.Collections.Generic;

namespace AppBrix.Web.Client.Impl
{
    internal sealed class DefaultHttpHeaders : Dictionary<string, IEnumerable<string>>, IHttpHeaders
    {
        public DefaultHttpHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            foreach (var header in headers)
            {
                this[header.Key] = header.Value;
            }
        }
    }
}
