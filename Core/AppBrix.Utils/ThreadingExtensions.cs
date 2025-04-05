// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Utils.Contracts;
using AppBrix.Utils.Impl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix;

/// <summary>
/// Used for storing commonly used threading extension methods.
/// </summary>
public static class ThreadingExtensions
{
    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlim"/>.
    /// Return a disposable <see cref="ISemaphoreReleaser"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/> object. 
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim"/>.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> timeout to wait, default value is to wait indefinitely.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A disposable <see cref="ISemaphoreReleaser"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/> object.</returns>
    public static ISemaphoreReleaser WaitRelease(this SemaphoreSlim semaphore, TimeSpan? timeout = null, CancellationToken token = default) =>
        semaphore.Wait(timeout ?? Timeout.InfiniteTimeSpan, token) ? new SemaphoreReleaser(semaphore) : SemaphoreReleaser.None;

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlim"/>.
    /// Return a disposable <see cref="ISemaphoreReleaser"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/> object. 
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim"/>.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> timeout to wait, default value is to wait indefinitely.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A disposable <see cref="ISemaphoreReleaser"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/> object.</returns>
    public static async Task<ISemaphoreReleaser> WaitAsyncRelease(this SemaphoreSlim semaphore, TimeSpan? timeout = null, CancellationToken token = default) =>
        await semaphore.WaitAsync(timeout ?? Timeout.InfiniteTimeSpan, token) ? new SemaphoreReleaser(semaphore) : SemaphoreReleaser.None;
}
