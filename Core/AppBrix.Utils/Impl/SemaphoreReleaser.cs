using AppBrix.Utils.Contracts;
using System.Threading;

namespace AppBrix.Utils.Impl;

internal sealed class SemaphoreReleaser : ISemaphoreReleaser
{
    #region Construction
    public SemaphoreReleaser(SemaphoreSlim semaphore)
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
    internal static readonly ISemaphoreReleaser None = new EmptySemaphoreReleaser();
    private readonly SemaphoreSlim semaphore;
    #endregion

    #region Private classes
    private sealed class EmptySemaphoreReleaser : ISemaphoreReleaser
    {
        public bool Success => false;
        public void Dispose() { }
    }
    #endregion
}
