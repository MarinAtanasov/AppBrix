// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using Microsoft.Framework.Caching.Distributed;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Caching
{
    internal class DefaultCache : ICache
    {
        #region Construction
        public DefaultCache(IApp app, IDistributedCache cache)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (cache == null)
                throw new ArgumentNullException("cache");

            this.app = app;
            this.cache = cache;
        }
        #endregion

        #region Public and overriden methods
        public void Connect()
        {
            this.cache.Connect();
        }

        public Task ConnectAsync()
        {
            return this.cache.ConnectAsync();
        }

        public T Get<T>(string key)
        {
            var bytes = this.cache.Get(key);
            return bytes != null ? this.GetSerializer().Deserialize<T>(bytes) : default(T);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var bytes = await this.cache.GetAsync(key);
            return bytes != null ? this.GetSerializer().Deserialize<T>(bytes) : default(T);
        }

        public void Refresh(string key)
        {
            this.cache.Refresh(key);
        }

        public Task RefreshAsync(string key)
        {
            return this.cache.RefreshAsync(key);
        }

        public void Remove(string key)
        {
            this.cache.Remove(key);
        }

        public Task RemoveAsync(string key)
        {
            return this.cache.RemoveAsync(key);
        }

        public void Set<T>(string key, T item)
        {
            var serialized = this.GetSerializer().Serialize<T>(item);
            this.cache.Set(key, serialized);
        }

        public Task SetAsync<T>(string key, T item)
        {
            var serialized = this.GetSerializer().Serialize<T>(item);
            return this.cache.SetAsync(key, serialized);
        }
        #endregion

        #region Private methods
        private ICacheSerializer GetSerializer()
        {
            return this.app.Get<ICacheSerializer>();
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        private readonly IDistributedCache cache;
        #endregion
    }
}
