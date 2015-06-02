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
    public interface IFactory
    {
        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to be returned by the factory.</typeparam>
        /// <param name="factory">The factory method.</param>
        void Register<T>(Func<T> factory);

        /// <summary>
        /// Returns an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <returns>An instance of an object of type T.</returns>
        T Get<T>();

        /// <summary>
        /// Returns an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to be returned.</param>
        /// <returns>An instance of an object of the specified type.</returns>
        object Get(Type type);
    }
}
