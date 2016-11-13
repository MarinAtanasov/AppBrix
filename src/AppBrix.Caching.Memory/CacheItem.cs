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
            this.AbsoluteExpiration = now.Add(absoluteExpiration);
            this.rollingExpirationSpan = rollingExpirationSpan;
            this.Created = now;
            this.LastAccessed = now;
        }
        #endregion

        #region Properties
        public object Item { get; }

        public DateTime Created { get; }

        public DateTime LastAccessed
        {
            get
            {
                return this.lastAccessed;
            }
            set
            {
                this.lastAccessed = value;
                this.RollingExpiration = this.LastAccessed.Add(this.rollingExpirationSpan);
            }
        }

        public DateTime AbsoluteExpiration { get; }

        public DateTime RollingExpiration { get; private set; }
        #endregion

        #region Public and overriden methods
        public void Dispose()
        {
            this.dispose?.Invoke();
        }

        public bool HasExpired(DateTime now)
        {
            return this.AbsoluteExpiration < now || this.RollingExpiration < now;
        }
        #endregion

        #region Private fields and constants
        private readonly Action dispose;
        private readonly TimeSpan rollingExpirationSpan;
        private DateTime lastAccessed;
        #endregion
    }
}
