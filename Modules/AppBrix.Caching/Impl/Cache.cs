// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Services;
using AppBrix.Lifecycle;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Caching.Impl;

internal sealed class Cache : ICache, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
    }

    public void Uninitialize()
    {
        this.app = null!;
    }
    #endregion

    #region ICache implementation
    public async Task<object?> Get(string key, Type type, CancellationToken token = default)
    {
        var bytes = await this.app.GetDistributedCache().GetAsync(key, token).ConfigureAwait(false);
        return bytes is null ? null : this.app.GetCacheSerializer().Deserialize(bytes, type);
    }

    public Task Refresh(string key, CancellationToken token = default) => this.app.GetDistributedCache().RefreshAsync(key, token);

    public Task Remove(string key, CancellationToken token = default) => this.app.GetDistributedCache().RemoveAsync(key, token);

    public Task Set(string key, object item, CancellationToken token = default) => this.app.GetDistributedCache().SetAsync(key, this.app.GetCacheSerializer().Serialize(item), token);
    #endregion

    #region Private fields and constants
    private IApp app = null!;
    #endregion
}
