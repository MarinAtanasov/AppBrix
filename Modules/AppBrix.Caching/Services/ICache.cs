// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Caching.Services;

/// <summary>
/// Defines a remote cache which holds serializable data objects.
/// </summary>
public interface ICache
{
	/// <summary>
	/// Refreshes the cache for a key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
	/// <returns></returns>
	Task Refresh(string key, CancellationToken token = default);

	/// <summary>
	/// Gets a cached object by its key.
	/// </summary>
	/// <typeparam name="T">The type of the object to get.</typeparam>
	/// <param name="key">The key which is used to store the object in the cache.</param>
	/// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
	/// <returns></returns>
	async Task<T> Get<T>(string key, CancellationToken token = default) => (T)(await this.Get(key, typeof(T), token).ConfigureAwait(false) ?? default(T))!;

	/// <summary>
	/// Gets a cached object by its key.
	/// </summary>
	/// <param name="key">The key which is used to store the object in the cache.</param>
	/// <param name="type">The type of the object to get.</param>
	/// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
	/// <returns></returns>
	Task<object?> Get(string key, Type type, CancellationToken token = default);

	/// <summary>
	/// Sets a cached object.
	/// </summary>
	/// <param name="key">The key which will be used when storing the object.</param>
	/// <param name="item">The object to be cached.</param>
	/// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
	/// <returns></returns>
	Task Set(string key, object item, CancellationToken token = default);

	/// <summary>
	/// Removes an object from the cache.
	/// </summary>
	/// <param name="key">The key to the object.</param>
	/// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
	/// <returns></returns>
	Task Remove(string key, CancellationToken token = default);
}
