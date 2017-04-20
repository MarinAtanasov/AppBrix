// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Caching.Memory
{
    internal sealed class CacheItem : IDisposable
    {
        #region Construction
        public CacheItem(object item, Action dispose, TimeSpan absoluteExpiration, TimeSpan rollingExpirationSpan, DateTime now)
        {
            this.Item = item;
            this.dispose = dispose;
            this.absoluteExpiration = now.Add(absoluteExpiration);
            this.rollingExpirationSpan = rollingExpirationSpan;
            this.UpdateLastAccessed(now);
        }
        #endregion

        #region Properties
        public object Item { get; }
        #endregion

        #region Public and overriden methods
        public void Dispose()
        {
            this.dispose?.Invoke();
        }

        public void UpdateLastAccessed(DateTime now)
        {
            this.rollingExpiration = now.Add(this.rollingExpirationSpan);
        }

        public bool HasExpired(DateTime now)
        {
            return this.absoluteExpiration < now || this.rollingExpiration < now;
        }
        #endregion

        #region Private fields and constants
        private readonly Action dispose;
        private readonly DateTime absoluteExpiration;
        private readonly TimeSpan rollingExpirationSpan;
        private DateTime rollingExpiration;
        #endregion
    }
}
