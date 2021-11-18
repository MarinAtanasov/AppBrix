// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Container;

/// <summary>
/// Registers objects and enables resolving of registered objects.
/// </summary>
public interface IContainer
{
    /// <summary>
    /// Registers an object as its type, parent types and interfaces.
    /// </summary>
    /// <param name="obj">The object to be registered. Required.</param>
    void Register(object obj);

    /// <summary>
    /// Returns the last registered object of a given type.
    /// </summary>
    /// <typeparam name="T">The type of the registered object.</typeparam>
    /// <returns>The last registered object.</returns>
    T Get<T>() where T : class => (T)this.Get(typeof(T));

    /// <summary>
    /// Returns the last registered object of a given type.
    /// </summary>
    /// <param name="type">The type of the registered object.</param>
    /// <returns>The last registered object.</returns>
    object Get(Type type);
}
