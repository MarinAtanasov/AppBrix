// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix
{
    public static class ContainerExtensions
    {
        #region IContainer extensions
        /// <summary>
        /// Gets the application's current container.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The registered container.</returns>
        public static IContainer GetContainer(this IApp app)
        {
            return ContainerExtensions.Containers[app];
        }

        /// <summary>
        /// Sets the application's current container.
        /// Pass in null to unregister the container.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="container">The container.</param>
        public static void SetContainer(this IApp app, IContainer container)
        {
            lock (ContainerExtensions.Containers)
            {
                if (container != null)
                {
                    ContainerExtensions.Containers[app] = container;
                }
                else if (ContainerExtensions.Containers.ContainsKey(app))
                {
                    ContainerExtensions.Containers.Remove(app);
                }
            }
        }

        /// <summary>
        /// Resolves an item by its type.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="type">The type of the object to be resolved.</param>
        /// <returns></returns>
        public static object Get(this IApp app, Type type)
        {
            return app.GetContainer().Get(type);
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
            return app.GetContainer().Get<T>();
        }

        /// <summary>
        /// Registers an object as the passed-in type, its parent types and interfaces.
        /// This method can be used when the type is known during compile time.
        /// </summary>
        /// <typeparam name="T">The type to be used as base upon registration. Cannot be "object".</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="obj">The object to be registered. Required.</param>
        /// <exception cref="ArgumentNullException">obj</exception>
        /// <exception cref="ArgumentException">T is of type object.</exception>
        public static void Register<T>(this IContainer container, T obj) where T : class
        {
            container.Register(obj, typeof(T));
        }

        /// <summary>
        /// Returns the last registered object of a given type.
        /// This method has lower performance than calling <see cref="IContainer.Get(Type)"/> and casting the result.
        /// </summary>
        /// <typeparam name="T">The type of the registered object.</typeparam>
        /// <exception cref="ArgumentException">No object of the specified type has been registered.</exception>
        /// <returns>The last registered object.</returns>
        public static T Get<T>(this IContainer container) where T : class
        {
            return (T)container.Get(typeof(T));
        }
        #endregion

        #region Private fields and constants
        private static readonly Dictionary<IApp, IContainer> Containers = new Dictionary<IApp, IContainer>();
        #endregion
    }
}
