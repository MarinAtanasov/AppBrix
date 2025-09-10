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
    /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim"/>. 
    /// Return a disposable <see cref="ISemaphoreLock"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/>. 
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim"/>.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A disposable <see cref="ISemaphoreLock"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/>.</returns>
    public static ISemaphoreLock Lock(this SemaphoreSlim semaphore, CancellationToken token = default) =>
        semaphore.Lock(Timeout.InfiniteTimeSpan, token);

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim"/>. 
    /// Return a disposable <see cref="ISemaphoreLock"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/>. 
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim"/>.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> timeout to wait.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A disposable <see cref="ISemaphoreLock"/> that can be used inside a using block to release the <see cref="SemaphoreSlim"/>.</returns>
    public static ISemaphoreLock Lock(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken token = default) =>
        semaphore.Wait(timeout, token) ? new SemaphoreLock(semaphore) : SemaphoreLock.None;

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlim"/>.
    /// Return a disposable <see cref="ISemaphoreLock"/> that can be used in a using block to release the <see cref="SemaphoreSlim"/>. 
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim"/>.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A disposable <see cref="ISemaphoreLock"/> that can be used in a using block to release the <see cref="SemaphoreSlim"/>.</returns>
    public static Task<ISemaphoreLock> AsyncLock(this SemaphoreSlim semaphore, CancellationToken token = default) =>
        semaphore.AsyncLock(Timeout.InfiniteTimeSpan, token);

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlim"/>.
    /// Return a disposable <see cref="ISemaphoreLock"/> that can be used in a using block to release the <see cref="SemaphoreSlim"/>. 
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim"/>.</param>
    /// <param name="timeout">A <see cref="TimeSpan"/> timeout to wait.</param>
    /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A disposable <see cref="ISemaphoreLock"/> that can be used in a using block to release the <see cref="SemaphoreSlim"/>.</returns>
    public static async Task<ISemaphoreLock> AsyncLock(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken token = default) =>
        await semaphore.WaitAsync(timeout, token) ? new SemaphoreLock(semaphore) : SemaphoreLock.None;
}
