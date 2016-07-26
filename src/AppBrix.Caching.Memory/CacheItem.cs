// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Caching.Memory
{
    internal sealed class CacheItem : IDisposable
    {
        public CacheItem(object item, Action dispose, TimeSpan absoluteExpiration, TimeSpan rollingExpirationSpan, DateTime now)
        {
            this.Item = item;
            this.dispose = dispose;
            this.Created = now;
            this.LastAccessed = now;
            var absoluteExpirationSpan = absoluteExpiration > TimeSpan.Zero ? absoluteExpiration : TimeSpan.FromDays(365);
            this.AbsoluteExpiration = this.Created.Add(absoluteExpirationSpan);
            this.rollingExpirationSpan = rollingExpirationSpan > TimeSpan.Zero ? rollingExpirationSpan : TimeSpan.FromDays(365);
            this.RollingExpiration = this.LastAccessed.Add(this.rollingExpirationSpan);
        }

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
                if (this.rollingExpirationSpan > TimeSpan.Zero)
                {
                    this.RollingExpiration = this.LastAccessed.Add(this.rollingExpirationSpan);
                }
            }
        }

        public DateTime AbsoluteExpiration { get; }

        public DateTime RollingExpiration { get; private set; }

        public bool HasExpired(DateTime now)
        {
            return this.AbsoluteExpiration < now || this.RollingExpiration < now;
        }

        public void Dispose()
        {
            this.dispose?.Invoke();
        }

        private readonly object item;
        private readonly Action dispose;
        private DateTime lastAccessed;
        private TimeSpan rollingExpirationSpan;
    }
}
