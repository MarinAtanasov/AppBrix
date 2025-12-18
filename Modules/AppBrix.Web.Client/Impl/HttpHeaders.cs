// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Web.Client.Contracts;
using System.Collections.Generic;

namespace AppBrix.Web.Client.Impl;

internal sealed class HttpHeaders : Dictionary<string, IEnumerable<string>>, IHttpHeaders
{
	public HttpHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
	{
		foreach (var header in headers)
		{
			this[header.Key] = header.Value;
		}
	}
}
