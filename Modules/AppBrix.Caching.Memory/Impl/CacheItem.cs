// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Caching.Memory.Impl;

internal sealed class CacheItem : IDisposable
{
    #region Construction
    public CacheItem(object item, Action? dispose, TimeSpan absoluteExpiration, TimeSpan slidingExpirationSpan, DateTime now)
    {
        this.Item = item;
        this.dispose = dispose;
        this.absoluteExpiration = now.Add(absoluteExpiration);
        this.slidingExpirationSpan = slidingExpirationSpan;
        this.UpdateLastAccessed(now);
    }
    #endregion

    #region Properties
    public object Item { get; }
    #endregion

    #region Public and overriden methods
    public void Dispose() => this.dispose?.Invoke();

    public void UpdateLastAccessed(DateTime now) => this.slidingExpiration = now.Add(this.slidingExpirationSpan);

    public bool HasExpired(DateTime now) => this.absoluteExpiration < now || this.slidingExpiration < now;
    #endregion

    #region Private fields and constants
    private readonly Action? dispose;
    private readonly DateTime absoluteExpiration;
    private readonly TimeSpan slidingExpirationSpan;
    private DateTime slidingExpiration;
    #endregion
}
