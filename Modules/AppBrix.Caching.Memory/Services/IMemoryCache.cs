// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Caching.Memory.Services;

/// <summary>
/// Defines an in-memory local cache.
/// This can be used for objects which are long running and should
/// be disposed after absolute or sliding expiration time.
/// </summary>
public interface IMemoryCache
{
	/// <summary>
	/// Gets a cached object by its key.
	/// </summary>
	/// <typeparam name="T">The type of the item to return.</typeparam>
	/// <param name="key">The key which is used to store the object in the cache.</param>
	/// <returns>The cached object. Returns null if no object is found.</returns>
	T Get<T>(object key) => (T)(this.Get(key) ?? default(T)!);

	/// <summary>
	/// Gets a cached object by its key.
	/// </summary>
	/// <param name="key">The key which is used to store the object in the cache.</param>
	/// <returns>The cached object. Returns null if no object is found.</returns>
	object? Get(object key);

	/// <summary>
	/// Stores an object in the cache.
	/// </summary>
	/// <param name="key">The key which will be used to store the item.</param>
	/// <param name="item">The item to be stored.</param>
	/// <param name="dispose">Optional action to be executed when the absolute or sliding expirations are reached.</param>
	/// <param name="absoluteExpiration">Absolute expiration time.</param>
	/// <param name="slidingExpiration">Sliding expiration time.</param>
	void Set(object key, object item, Action? dispose = null, TimeSpan absoluteExpiration = default, TimeSpan slidingExpiration = default);

	/// <summary>
	/// Removes a cached item by its key.
	/// </summary>
	/// <param name="key">The key of the stored item.</param>
	/// <returns>Returns if the object was found in the cache.</returns>
	bool Remove(object key);
}
