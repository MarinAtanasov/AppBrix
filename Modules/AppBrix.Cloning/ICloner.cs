// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Cloning
{
    /// <summary>
    /// Creates deep or shallow copies of objects.
    /// </summary>
    public interface ICloner
    {
        /// <summary>
        /// Creates a deep copy of the specified object.
        /// </summary>
        /// <param name="obj">The object to be copied</param>
        /// <returns>A deep copy of the specified object.</returns>
        object DeepCopy(object obj);

        /// <summary>
        /// Creates a shallow copy of the specified object.
        /// </summary>
        /// <param name="obj">The object to be copied</param>
        /// <returns>A shallow copy of the specified object.</returns>
        object ShallowCopy(object obj);
    }
}
