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
        /// Registers an object as its type, parent types and interfaces.
        /// </summary>
        /// <param name="obj">The object to be registered. Required.</param>
        void Register(object obj);

        /// <summary>
        /// Returns the last registered object of a given type.
        /// </summary>
        /// <param name="type">The type of the registered object.</param>
        /// <returns>The last registered object.</returns>
        object Get(Type type);

        /// <summary>
        /// Resolves all registered objects in the order in which they were registered.
        /// </summary>
        /// <returns>All registered objects.</returns>
        IEnumerable<object> GetAll();
    }
}
