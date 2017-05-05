// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Web.Client
{
    /// <summary>
    /// HTTP REST headers.
    /// </summary>
    public interface IHttpHeaders : IDictionary<string, IEnumerable<string>>
    {
    }
}
