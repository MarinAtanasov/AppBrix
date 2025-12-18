// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Utils.Contracts;
using System.Threading;

namespace AppBrix.Utils.Impl;

internal sealed class SemaphoreLock : ISemaphoreLock
{
	#region Construction
	public SemaphoreLock(SemaphoreSlim semaphore)
	{
		this.semaphore = semaphore;
	}
	#endregion

	#region Properties
	public bool Success => true;
	#endregion

	#region Public and overriden methods
	public void Dispose() => this.semaphore.Release();
	#endregion

	#region Private fields and constants
	internal static readonly ISemaphoreLock None = new FailedSemaphoreLock();
	private readonly SemaphoreSlim semaphore;
	#endregion

	#region Private classes
	private sealed class FailedSemaphoreLock : ISemaphoreLock
	{
		public bool Success => false;
		public void Dispose() { }
	}
	#endregion
}
