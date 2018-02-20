// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Container;
using System;
using System.Linq;

namespace AppBrix
{
    public static class ContainerExtensions
    {
        #region IContainer extensions
        /// <summary>
        /// Resolves an item by its type.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="type">The type of the object to be resolved.</param>
        /// <returns></returns>
        public static object Get(this IApp app, Type type)
        {
            return app.Container.Get(type);
        }

        /// <summary>
        /// Resolves an item by its type.
        /// This method has lower performance than calling <see cref="Get(IApp, Type)"/> and casting the result.
        /// </summary>
        /// <typeparam name="T">The type of the object to be resolved.</typeparam>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static T Get<T>(this IApp app) where T : class
        {
            return app.Container.Get<T>();
        }

        /// <summary>
        /// Registers an object as the passed-in type, its parent types and interfaces.
        /// This method can be used when the type is known during compile time.
        /// </summary>
        /// <typeparam name="T">The type to be used as base upon registration. Cannot be "object".</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="obj">The object to be registered. Required.</param>
        public static void Register<T>(this IContainer container, T obj) where T : class
        {
            container.Register(obj, typeof(T));
        }

        /// <summary>
        /// Returns the last registered object of a given type.
        /// This method has lower performance than calling <see cref="IContainer.Get(Type)"/> and casting the result.
        /// </summary>
        /// <typeparam name="T">The type of the registered object.</typeparam>
        /// <returns>The last registered object.</returns>
        public static T Get<T>(this IContainer container) where T : class
        {
            return (T)container.Get(typeof(T));
        }
        #endregion
    }
}
