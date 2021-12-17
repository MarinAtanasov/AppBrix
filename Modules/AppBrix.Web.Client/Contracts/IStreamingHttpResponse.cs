// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.IO;

namespace AppBrix.Web.Client.Contracts;

/// <summary>
/// HTTP response object which is returned by <see cref="IHttpRequest"/> with content as <see cref="Stream"/>.
/// </summary>
public interface IStreamingHttpResponse : IHttpResponse<Stream>, IDisposable, IAsyncDisposable
{
}
