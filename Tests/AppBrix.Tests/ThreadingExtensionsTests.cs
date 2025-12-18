// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Tests;

[TestClass]
public sealed class UtilsExtensions : TestsBase
{
	#region Tests
	[Test, Functional]
	public void TestLockSuccess()
	{
		var semaphore = new SemaphoreSlim(1);

		var semaphoreLock = semaphore.Lock();
		this.Assert(semaphoreLock.Success, "the lock should succeed");
		this.Assert(semaphore.CurrentCount == 0, "the current count should decrease after successful lock");

		semaphoreLock.Dispose();

		this.Assert(semaphore.CurrentCount == 1, "the current count should increase after lock release");
	}

	[Test, Functional]
	public void TestLockFail()
	{
		var semaphore = new SemaphoreSlim(0);

		var semaphoreLock = semaphore.Lock(TimeSpan.Zero);
		this.Assert(semaphoreLock.Success == false, "the lock should fail");
		this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed lock");

		semaphoreLock.Dispose();

		this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed lock release");
	}

	[Test, Functional]
	public async Task TestAsyncLockSuccess()
	{
		var semaphore = new SemaphoreSlim(1);

		var semaphoreLock = await semaphore.AsyncLock();
		this.Assert(semaphoreLock.Success, "the lock should succeed");
		this.Assert(semaphore.CurrentCount == 0, "the current count should decrease after successful lock");

		semaphoreLock.Dispose();

		this.Assert(semaphore.CurrentCount == 1, "the current count should increase after lock release");
	}

	[Test, Functional]
	public async Task TestAsyncLockFail()
	{
		var semaphore = new SemaphoreSlim(0);

		var semaphoreLock = await semaphore.AsyncLock(TimeSpan.Zero);
		this.Assert(semaphoreLock.Success == false, "the lock should fail");
		this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed lock");

		semaphoreLock.Dispose();

		this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed lock release");
	}

	[Test, Functional]
	public async Task TestAsyncLockQueue()
	{
		var semaphore = new SemaphoreSlim(0);

		var semaphoreLockTask = semaphore.AsyncLock();
		this.Assert(semaphoreLockTask.IsCompleted == false, "the task should not complete while waiting in queue");

		semaphore.Release();

		using var semaphoreLock = await semaphoreLockTask;
		this.Assert(semaphoreLock.Success, "the lock should succeed");
	}
	#endregion
}
