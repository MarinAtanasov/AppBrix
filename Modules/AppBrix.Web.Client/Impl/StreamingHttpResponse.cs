// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Web.Client.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AppBrix.Web.Client.Impl;

internal sealed class StreamingHttpResponse : IStreamingHttpResponse
{
	#region Construction
	public StreamingHttpResponse(IHttpHeaders headers, Stream content, int statusCode, string reasonPhrase, Version version)
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

	public Stream Content { get; }

	public int StatusCode { get; }

	public string ReasonPhrase { get; }

	public Version Version { get; }
	#endregion

	#region Public and overriden methods
	public void Dispose() => this.Content.Dispose();

	public ValueTask DisposeAsync() => this.Content.DisposeAsync();
	#endregion
}
