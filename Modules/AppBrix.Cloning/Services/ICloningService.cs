﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Cloning.Services;

/// <summary>
/// Service that creates deep and shallow copies of objects.
/// </summary>
public interface ICloningService
{
    /// <summary>
    /// Creates a deep copy of the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to be copied</param>
    /// <returns>A deep copy of the specified object.</returns>
    T DeepCopy<T>(T obj);

    /// <summary>
    /// Creates a shallow copy of the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to be copied</param>
    /// <returns>A shallow copy of the specified object.</returns>
    T ShallowCopy<T>(T obj);
}
