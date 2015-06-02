// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;

namespace AppBrix
{
    public static class ResolverExtensions
    {
        #region IResolver extensions
        /// <summary>
        /// Registers an object as the passed-in type, its parent types and interfaces.
        /// This method can be used when the type is known during compile time.
        /// </summary>
        /// <typeparam name="T">The type to be used as base upon registration. Cannot be "object".</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="obj">The object to be registered. Required.</param>
        /// <exception cref="ArgumentNullException">obj</exception>
        /// <exception cref="ArgumentException">T is of type object.</exception>
        public static void Register<T>(this IResolver resolver, T obj) where T : class
        {
            resolver.Register(obj, typeof(T));
        }
        #endregion
    }
}
