// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Factory
{
    /// <summary>
    /// Registers and executes creation of objects.
    /// </summary>
    public interface IFactoryService
    {
        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="type">The type to be returned by the factory.</param>
        void Register(IFactory<object> factory, Type type);
        
        /// <summary>
        /// Returns the registered factory for the specified type.
        /// </summary>
        /// <param name="type">The type of the object to be returned.</param>
        /// <returns>An instance of an object of the specified type.</returns>
        IFactory<object> GetFactory(Type type);
    }
}
