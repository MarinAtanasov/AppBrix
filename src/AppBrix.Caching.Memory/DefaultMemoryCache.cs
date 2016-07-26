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
            this.cache.Keys.ToList().ForEach(this.Remove);
            this.app = null;
        }
        #endregion

        #region ILocalCache implementation
        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
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

        public void Set(string key, object item, Action dispose = null, TimeSpan absoluteExpiration = default(TimeSpan), TimeSpan rollingExpiration = default(TimeSpan))
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (absoluteExpiration < TimeSpan.Zero)
                throw new ArgumentException($"Negative absolute expiration: {absoluteExpiration}.");
            if (rollingExpiration < TimeSpan.Zero)
                throw new ArgumentException($"Negative rolling expiration: {rollingExpiration}.");

            lock (this.cache)
            {
                var existing = this.GetInternal(key);
                existing?.Dispose();
                var now = this.app.GetTime();
                this.cache[key] = new CacheItem(item, dispose, absoluteExpiration, rollingExpiration, now);
            }
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
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
        private CacheItem GetInternal(string key)
        {
            CacheItem result;

            this.cache.TryGetValue(key, out result);
            if (result != null)
            {
                result.LastAccessed = this.app.GetTime();
            }

            return result;
        }

        private void RemoveExpiredEntries(object unused)
        {
            var now = this.app.GetTime();
            IEnumerable<KeyValuePair<string, CacheItem>> expired;
            lock (this.cache)
            {
                expired = this.cache.Where(x => x.Value.HasExpired(now)).ToList();
                foreach (var item in expired)
                {
                    this.cache.Remove(item.Key);
                }
            }
            foreach (var item in expired)
            {
                item.Value.Dispose();
            }

            this.expirationTimer.Change(this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck, TimeSpan.FromMilliseconds(-1));
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<string, CacheItem> cache = new Dictionary<string, CacheItem>();
        private IApp app;
        private Timer expirationTimer;
        #endregion
    }
}
