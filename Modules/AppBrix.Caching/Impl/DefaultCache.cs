// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
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
        public async Task<object?> Get(string key, Type type)
        {
            var bytes = await this.GetCache().GetAsync(key).ConfigureAwait(false);
            return bytes != null ? this.GetSerializer().Deserialize(bytes, type) : null;
        }
        
        public Task Refresh(string key) => this.GetCache().RefreshAsync(key);
        
        public Task Remove(string key) => this.GetCache().RemoveAsync(key);
        
        public Task Set(string key, object item) => this.GetCache().SetAsync(key, this.GetSerializer().Serialize(item));
        #endregion

        #region Private methods
        private IDistributedCache GetCache() => (IDistributedCache)this.app.Get(typeof(IDistributedCache));

        private ICacheSerializer GetSerializer() => (ICacheSerializer)this.app.Get(typeof(ICacheSerializer));
        #endregion

        #region Private fields and constants
        #nullable disable
        private IApp app;
        #nullable restore
        #endregion
    }
}
