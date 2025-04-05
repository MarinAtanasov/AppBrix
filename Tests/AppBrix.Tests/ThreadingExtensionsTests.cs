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
    public void TestWaitReleaseSuccess()
    {
        var semaphore = new SemaphoreSlim(1);

        var waitRelease = semaphore.WaitRelease();
        this.Assert(waitRelease.Success, "the wait should succeed");
        this.Assert(semaphore.CurrentCount == 0, "the current count should decrease after successful wait");

        waitRelease.Dispose();

        this.Assert(semaphore.CurrentCount == 1, "the current count should increase after release");
    }

    [Test, Functional]
    public void TestWaitReleaseFail()
    {
        var semaphore = new SemaphoreSlim(0);

        var waitRelease = semaphore.WaitRelease(TimeSpan.Zero);
        this.Assert(waitRelease.Success == false, "the wait should fail");
        this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed wait");

        waitRelease.Dispose();

        this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed wait release");
    }

    [Test, Functional]
    public async Task TestWaitAsyncReleaseSuccess()
    {
        var semaphore = new SemaphoreSlim(1);

        var waitRelease = await semaphore.WaitAsyncRelease();
        this.Assert(waitRelease.Success, "the wait should succeed");
        this.Assert(semaphore.CurrentCount == 0, "the current count should decrease after successful wait");

        waitRelease.Dispose();

        this.Assert(semaphore.CurrentCount == 1, "the current count should increase after release");
    }

    [Test, Functional]
    public async Task TestWaitAsyncReleaseFail()
    {
        var semaphore = new SemaphoreSlim(0);

        var waitRelease = await semaphore.WaitAsyncRelease(TimeSpan.Zero);
        this.Assert(waitRelease.Success == false, "the wait should fail");
        this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed wait");

        waitRelease.Dispose();

        this.Assert(semaphore.CurrentCount == 0, "the current count should not change after failed wait release");
    }

    [Test, Functional]
    public async Task TestWaitAsyncReleaseQueue()
    {
        var semaphore = new SemaphoreSlim(0);

        var waitReleaseTask = semaphore.WaitAsyncRelease();
        this.Assert(waitReleaseTask.IsCompleted == false, "the task should not complete while waiting in queue");

        semaphore.Release();

        using var waitRelease = await waitReleaseTask;
        this.Assert(waitRelease.Success, "the wait should succeed");
    }
    #endregion
}
