// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Caching.Impl
{
    internal sealed class DefaultCache : ICache, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.app = null;
        }
        #endregion

        #region ICache implementation
        public async Task<object> Get(string key, Type type)
        {
            var bytes = await this.GetCache().GetAsync(key);
            return bytes != null ? this.GetSerializer().Deserialize(bytes, type) : null;
        }
        
        public Task Refresh(string key)
        {
            return this.GetCache().RefreshAsync(key);
        }
        
        public Task Remove(string key)
        {
            return this.GetCache().RemoveAsync(key);
        }
        
        public Task Set(string key, object item)
        {
            var serialized = this.GetSerializer().Serialize(item);
            return this.GetCache().SetAsync(key, serialized);
        }
        #endregion

        #region Private methods
        private IDistributedCache GetCache()
        {
            return (IDistributedCache)this.app.Get(typeof(IDistributedCache));
        }

        private ICacheSerializer GetSerializer()
        {
            return (ICacheSerializer)this.app.Get(typeof(ICacheSerializer));
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
