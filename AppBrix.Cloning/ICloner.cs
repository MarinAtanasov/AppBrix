// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Cloning
{
    /// <summary>
    /// Creates deep copies of objects.
    /// </summary>
    public interface ICloner
    {
        /// <summary>
        /// Creates a deep copy of the specified object.
        /// </summary>
        /// <typeparam name="T">The type to be returned. Will be used only for casting the final result.</typeparam>
        /// <param name="obj">The object to be cloned</param>
        /// <returns>A deep copy of the specified object.</returns>
        T Clone<T>(T obj);
    }
}
