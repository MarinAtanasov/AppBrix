// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Cloning
{
    /// <summary>
    /// Creates deep or shalow copies of objects.
    /// </summary>
    public interface ICloner
    {
        /// <summary>
        /// Creates a deep copy of the specified object.
        /// </summary>
        /// <typeparam name="T">The type to be returned.</typeparam>
        /// <param name="obj">The object to be copied</param>
        /// <returns>A deep copy of the specified object.</returns>
        T DeepCopy<T>(T obj);

        /// <summary>
        /// Creates a shalow copy of the specified object.
        /// </summary>
        /// <typeparam name="T">The type to be returned.</typeparam>
        /// <param name="obj">The object to be copied</param>
        /// <returns>A shalow copy of the specified object.</returns>
        T ShalowCopy<T>(T obj);
    }
}
