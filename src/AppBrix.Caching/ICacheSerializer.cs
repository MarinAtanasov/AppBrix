// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Caching
{
    /// <summary>
    /// Serializes and deserializes items when accessing the cache.
    /// </summary>
    public interface ICacheSerializer
    {
        /// <summary>
        /// Serializes an item to <see cref="T:byte[]"/>.
        /// </summary>
        /// <param name="type">The type of the item.</param>
        /// <param name="item">The item.</param>
        /// <returns>The byte array representation of the item.</returns>
        byte[] Serialize(Type type, object item);

        /// <summary>
        /// Deserializes an item from a byte array.
        /// </summary>
        /// <param name="type">The type of the item.</param>
        /// <param name="serialized">The byte array representation of the item.</param>
        /// <returns>The deserialized item.</returns>
        object Deserialize(Type type, byte[] serialized);
    }
}
