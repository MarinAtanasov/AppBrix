﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Memory.Configuration;
using AppBrix.Caching.Memory.Events;
using AppBrix.Caching.Memory.Services;
using AppBrix.Events.Schedule.Contracts;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;

namespace AppBrix.Caching.Memory.Impl;

internal sealed class MemoryCache : IMemoryCache, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
        this.app.GetEventHub().Subscribe<MemoryCacheCleanup>(this.MemoryCacheCleanup);
        this.config = this.app.ConfigService.GetMemoryCachingConfig();
        this.cleanupScheduledEventArgs = this.app.GetTimerScheduledEventHub().Schedule(this.cleanupEventArgs, this.config.ExpirationCheck);
    }

    public void Uninitialize()
    {
        lock (this.cleanupEventArgs)
        {
            if (this.cleanupScheduledEventArgs is not null)
            {
                this.app.GetTimerScheduledEventHub().Unschedule(this.cleanupScheduledEventArgs);
                this.cleanupScheduledEventArgs = null!;
            }
        }

        lock (this.cache)
        {
            this.app.GetEventHub().Unsubscribe<MemoryCacheCleanup>(this.MemoryCacheCleanup);

            this.keysToRemove.AddRange(this.cache.Keys);
            this.RemoveItemsByKeys();
            this.config = null!;
            this.app = null!;
        }
    }
    #endregion

    #region IMemoryCache implementation
    public object? Get(object key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        CacheItem? cacheItem;
        lock (this.cache)
        {
            if (this.cache.TryGetValue(key, out cacheItem))
                cacheItem.UpdateLastAccessed(this.app.GetTime());
        }
        return cacheItem?.Item;
    }

    public void Set(object key, object item, Action? dispose = null, TimeSpan absoluteExpiration = default, TimeSpan slidingExpiration = default)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));
        if (item is null)
            throw new ArgumentNullException(nameof(item));
        if (absoluteExpiration < TimeSpan.Zero)
            throw new ArgumentException($"Negative absolute expiration: {absoluteExpiration}.");
        if (slidingExpiration < TimeSpan.Zero)
            throw new ArgumentException($"Negative sliding expiration: {slidingExpiration}.");

        lock (this.cache)
        {
            this.cache.Remove(key, out var oldItem);

            this.cache.Add(key, new CacheItem(item, dispose,
                absoluteExpiration > TimeSpan.Zero ? absoluteExpiration : this.config.DefaultAbsoluteExpiration,
                slidingExpiration > TimeSpan.Zero ? slidingExpiration : this.config.DefaultSlidingExpiration,
                this.app.GetTime()));

            oldItem?.Dispose();
        }
    }

    public bool Remove(object key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        lock (this.cache)
        {
            if (this.cache.Remove(key, out var item))
            {
                item.Dispose();
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Private methods
    private void MemoryCacheCleanup(MemoryCacheCleanup _)
    {
        lock (this.cache)
        {
            if (this.app is not null)
            {
                var now = this.app.GetTime();
                foreach (var item in this.cache)
                {
                    if (item.Value.HasExpired(now))
                        this.keysToRemove.Add(item.Key);
                }

                this.RemoveItemsByKeys();
            }
        }

        lock (this.cleanupEventArgs)
        {
            if (this.cleanupScheduledEventArgs is not null)
            {
                this.cleanupScheduledEventArgs = this.app!.GetTimerScheduledEventHub().Schedule(this.cleanupEventArgs, this.config.ExpirationCheck);
            }
        }
    }

    private void RemoveItemsByKeys()
    {
        for (var i = 0; i < this.keysToRemove.Count; i++)
        {
            if (this.cache.Remove(this.keysToRemove[i], out var item))
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception)
                {
                    // Ignore error and continue disposing the rest of the items.
                }
            }
        }

        this.keysToRemove.Clear();
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<object, CacheItem> cache = new Dictionary<object, CacheItem>();
    private readonly List<object> keysToRemove = new List<object>();
    private readonly MemoryCacheCleanup cleanupEventArgs = new MemoryCacheCleanup();
    private IApp app = null!;
    private IScheduledEvent<MemoryCacheCleanup> cleanupScheduledEventArgs = null!;
    private MemoryCachingConfig config = null!;
    #endregion
}
