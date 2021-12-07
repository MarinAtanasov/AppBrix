// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Factory.Contracts;

/// <summary>
/// Creates objects of a given type.
/// </summary>
/// <typeparam name="T">The type of the objects to create.</typeparam>
public interface IFactory<out T>
{
    /// <summary>
    /// Returns an object from the factory.
    /// </summary>
    /// <returns>An instance of an object.</returns>
    T Get();
}
