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
                throw new ArgumentNullException(nameof(app));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            this.app = app;
            this.cache = cache;
        }
        #endregion

        #region Public and overriden methods
        public Task Connect()
        {
            return this.cache.ConnectAsync();
        }
        
        public async Task<T> Get<T>(string key)
        {
            var bytes = await this.cache.GetAsync(key);
            return bytes != null ? this.GetSerializer().Deserialize<T>(bytes) : default(T);
        }
        
        public Task Refresh(string key)
        {
            return this.cache.RefreshAsync(key);
        }
        
        public Task Remove(string key)
        {
            return this.cache.RemoveAsync(key);
        }
        
        public Task Set<T>(string key, T item)
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
