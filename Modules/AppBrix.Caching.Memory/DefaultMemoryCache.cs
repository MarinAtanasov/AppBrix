// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Caching.Memory.Configuration;
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
            var timeout = this.GetConfig().ExpirationCheck;
            lock (this.cache)
            {
                this.expirationTimer = new Timer(this.RemoveExpiredEntries, null, timeout, Timeout.InfiniteTimeSpan);
            }
        }

        public void Uninitialize()
        {
            lock (this.cache)
            {
                this.expirationTimer?.Dispose();
                this.expirationTimer = null;
                this.cache.Keys.ToList().ForEach(this.Remove);
            }

            this.app = null;
        }
        #endregion

        #region IMemoryCache implementation
        public object Get(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CacheItem cacheItem;
            lock (this.cache)
            {
                cacheItem = this.GetInternal(key);
                cacheItem?.UpdateLastAccessed(this.app.GetTime());
            }
            return cacheItem?.Item;
        }

        public void Set(object key, object item, Action dispose = null, TimeSpan absoluteExpiration = default(TimeSpan), TimeSpan slidingExpiration = default(TimeSpan))
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (absoluteExpiration < TimeSpan.Zero)
                throw new ArgumentException($"Negative absolute expiration: {absoluteExpiration}.");
            if (slidingExpiration < TimeSpan.Zero)
                throw new ArgumentException($"Negative sliding expiration: {slidingExpiration}.");

            var config = this.GetConfig();
            CacheItem oldItem;
            lock (this.cache)
            {
                oldItem = this.GetInternal(key);
                this.cache[key] = new CacheItem(item, dispose,
                    absoluteExpiration > TimeSpan.Zero ? absoluteExpiration : config.DefaultAbsoluteExpiration,
                    slidingExpiration > TimeSpan.Zero ? slidingExpiration : config.DefaultSlidingExpiration,
                    this.app.GetTime());
            }
            oldItem?.Dispose();
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
            this.cache.TryGetValue(key, out var result);
            return result;
        }

        private void RemoveExpiredEntries(object unused = null)
        {
            List<KeyValuePair<object, CacheItem>> toRemove;
            lock (this.cache)
            {
                if (this.expirationTimer == null)
                    return; // Unintialized

                var now = this.app.GetTime();

                toRemove = this.cache.Where(x => x.Value.HasExpired(now)).ToList();
                toRemove.ForEach(x => this.cache.Remove(x.Key));
                this.expirationTimer.Change(this.GetConfig().ExpirationCheck, Timeout.InfiniteTimeSpan);
            }
            toRemove.ForEach(x => this.RunSafe(x.Value.Dispose));
        }

        private void RunSafe(Action action)
        {
            try { action(); }
            catch (Exception) { }
        }

        private MemoryCachingConfig GetConfig()
        {
            return (MemoryCachingConfig)this.app.ConfigService.Get(DefaultMemoryCache.ConfigType);
        }
        #endregion

        #region Private fields and constants
        private static readonly Type ConfigType = typeof(MemoryCachingConfig);
        private readonly IDictionary<object, CacheItem> cache = new Dictionary<object, CacheItem>();
        private IApp app;
        private Timer expirationTimer;
        #endregion
    }
}
