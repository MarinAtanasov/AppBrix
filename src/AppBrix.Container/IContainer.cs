// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Container
{
    /// <summary>
    /// Registers objects and enables resolving of registered objects.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Registers an object as the passed-in type, its parent types and interfaces.
        /// This method can be used when the type is not known during compile time.
        /// </summary>
        /// <param name="obj">The object to be registered. Required.</param>
        /// <param name="type">The type to be used as base upon registration. Required. Cannot be "object".</param>
        /// <exception cref="ArgumentNullException">obj, type</exception>
        /// <exception cref="ArgumentException">T is of type object, obj is not of type T, obj already registered.</exception>
        void Register(object obj, Type type);

        /// <summary>
        /// Returns the last registered object of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the registered object.</typeparam>
        /// <exception cref="ArgumentException">No object of the specified type has been registered.</exception>
        /// <returns>The last registered object.</returns>
        T Get<T>() where T : class;

        /// <summary>
        /// Resolves all registered objects in the order in which they were registered.
        /// </summary>
        /// <returns>All registered objects.</returns>
        IEnumerable<object> GetAll();
    }
}
