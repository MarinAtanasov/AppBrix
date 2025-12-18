// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Web.Client.Impl;

internal sealed class StreamingHttpResponseContent : Stream, IDisposable
{
	#region Construction
	public StreamingHttpResponseContent(HttpResponseMessage response, Stream stream)
	{
		this.response = response;
		this.stream = stream;
	}
	#endregion

	#region Properties
	public override bool CanRead => this.stream.CanRead;

	public override bool CanSeek => this.stream.CanSeek;

	public override bool CanTimeout => this.stream.CanTimeout;

	public override bool CanWrite => this.stream.CanWrite;

	public override long Length => this.stream.Length;

	public override long Position
	{
		get => this.stream.Position;
		set => this.stream.Position = value;
	}

	public override int ReadTimeout
	{
		get => this.stream.ReadTimeout;
		set => this.stream.ReadTimeout = value;
	}

	public override int WriteTimeout
	{
		get => this.stream.WriteTimeout;
		set => this.stream.WriteTimeout = value;
	}
	#endregion

	#region Public and overriden methods
	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => this.stream.BeginRead(buffer, offset, count, callback, state);

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => this.stream.BeginWrite(buffer, offset, count, callback, state);

	public override void Close() => this.stream.Close();

	public override void CopyTo(Stream destination, int bufferSize) => this.stream.CopyTo(destination, bufferSize);

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => this.stream.CopyToAsync(destination, bufferSize, cancellationToken);

	public new void Dispose()
	{
		base.Dispose();
		this.stream.Dispose();
		this.response.Dispose();
	}

	public override async ValueTask DisposeAsync()
	{
		await base.DisposeAsync().ConfigureAwait(false);
		await this.stream.DisposeAsync().ConfigureAwait(false);
		this.response.Dispose();
	}

	public override int EndRead(IAsyncResult asyncResult) => this.stream.EndRead(asyncResult);

	public override void EndWrite(IAsyncResult asyncResult) => this.stream.EndWrite(asyncResult);

	public override bool Equals(object? obj) => this.stream.Equals(obj);

	public override void Flush() => this.stream.Flush();

	public override Task FlushAsync(CancellationToken cancellationToken) => this.stream.FlushAsync(cancellationToken);

	public override int GetHashCode() => this.stream.GetHashCode();

	public override int Read(Span<byte> buffer) => this.stream.Read(buffer);

	public override int Read(byte[] buffer, int offset, int count) => this.stream.Read(buffer, offset, count);

	public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => this.stream.ReadAsync(buffer, cancellationToken);

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => this.stream.ReadAsync(buffer, offset, count, cancellationToken);

	public override int ReadByte() => this.stream.ReadByte();

	public override long Seek(long offset, SeekOrigin origin) => this.stream.Seek(offset, origin);

	public override void SetLength(long value) => this.stream.SetLength(value);

	public override string? ToString() => this.stream.ToString();

	public override void Write(ReadOnlySpan<byte> buffer) => this.stream.Write(buffer);

	public override void Write(byte[] buffer, int offset, int count) => this.stream.Write(buffer, offset, count);

	public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => this.stream.WriteAsync(buffer, cancellationToken);

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => this.stream.WriteAsync(buffer, offset, count, cancellationToken);

	public override void WriteByte(byte value) => this.stream.WriteByte(value);
	#endregion

	#region Private fields and constants
	private readonly HttpResponseMessage response;
	private readonly Stream stream;
	#endregion
}
