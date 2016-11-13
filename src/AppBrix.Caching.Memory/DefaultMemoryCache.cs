// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AppBrix.Caching.Memory
{
    internal sealed class DefaultMemoryCache : IMemoryCache, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
            var timeout = this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck;
            this.expirationTimer = new Timer(this.RemoveExpiredEntries, null, timeout, TimeSpan.FromMilliseconds(-1));
        }

        public void Uninitialize()
        {
            this.expirationTimer?.Dispose();
            this.expirationTimer = null;

            lock (this.cache)
            {
                this.cache.Keys.ToList().ForEach(this.Remove);
            }

            this.app = null;
        }
        #endregion

        #region ILocalCache implementation
        public object Get(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CacheItem cacheItem;
            lock (this.cache)
            {
                cacheItem = this.GetInternal(key);
                if (cacheItem != null)
                {
                    cacheItem.LastAccessed = this.app.GetTime();
                }
            }
            return cacheItem?.Item;
        }

        public void Set(object key, object item, Action dispose = null, TimeSpan absoluteExpiration = default(TimeSpan), TimeSpan rollingExpiration = default(TimeSpan))
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (absoluteExpiration < TimeSpan.Zero)
                throw new ArgumentException($"Negative absolute expiration: {absoluteExpiration}.");
            if (rollingExpiration < TimeSpan.Zero)
                throw new ArgumentException($"Negative rolling expiration: {rollingExpiration}.");

            var config = this.app.GetConfig<MemoryCachingConfig>();
            lock (this.cache)
            {
                this.GetInternal(key)?.Dispose();
                this.cache[key] = new CacheItem(item, dispose,
                    absoluteExpiration > TimeSpan.Zero ? absoluteExpiration : config.DefaultAbsoluteExpiration,
                    rollingExpiration > TimeSpan.Zero ? rollingExpiration : config.DefaultRollingExpiration,
                    this.app.GetTime());
            }
        }

        public void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CacheItem item;

            lock (this.cache)
            {
                item = this.GetInternal(key);
                if (item != null)
                {
                    this.cache.Remove(key);
                }
            }

            item?.Dispose();
        }
        #endregion

        #region Private methods
        private CacheItem GetInternal(object key)
        {
            CacheItem result;
            this.cache.TryGetValue(key, out result);
            return result;
        }

        private void RemoveExpiredEntries(object unused)
        {
            var now = this.app.GetTime();

            lock (this.cache)
            {
                try
                {
                    foreach (var item in this.cache.Where(x => x.Value.HasExpired(now)).ToList())
                    {
                        this.cache.Remove(item.Key);
                        item.Value.Dispose();
                    }
                }
                finally
                {
                    this.expirationTimer.Change(this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck, TimeSpan.FromMilliseconds(-1));
                }
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<object, CacheItem> cache = new Dictionary<object, CacheItem>();
        private IApp app;
        private Timer expirationTimer;
        #endregion
    }
}
