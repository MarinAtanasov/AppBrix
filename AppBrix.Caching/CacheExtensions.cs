// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;

namespace AppBrix
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Gets the currently registered cache.
        /// </summary>
        /// <param name="app">The currently running application.</param>
        /// <returns>The cache.</returns>
        public static ICache GetCache(this IApp app)
        {
            return app.Get<ICache>();
        }


        /// <summary>
        /// Serializes an item to <see cref="T:byte[]"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <param name="item">The item to be serialized.</param>
        /// <returns>The byte array representation of the item.</returns>
        public static byte[] Serialize<T>(this ICacheSerializer serializer, T item)
        {
            return serializer.Serialize(typeof(T), item);
        }
        
        /// <summary>
        /// Deserializes an item from a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <param name="serialized">The item to be deserialized.</param>
        /// <returns>The deserialized item.</returns>
        public static T Deserialize<T>(this ICacheSerializer serializer, byte[] serialized)
        {
            return (T)serializer.Deserialize(typeof(T), serialized);
        }
    }
}
