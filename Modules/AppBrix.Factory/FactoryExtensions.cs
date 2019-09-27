// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Factory;
using AppBrix.Factory.Impl;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix factories.
    /// </summary>
    public static class FactoryExtensions
    {
        /// <summary>
        /// Gets the currently loaded factory service.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The factory.</returns>
        public static IFactoryService GetFactoryService(this IApp app) => (IFactoryService)app.Get(typeof(IFactoryService));

        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to be returned by the factory.</typeparam>
        /// <param name="factoryService">The factory service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public static void Register<T>(this IFactoryService factoryService, Func<T> factoryMethod) where T : class
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            factoryService.Register(factoryMethod, typeof(T));
        }

        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <param name="factoryService">The factory service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="type">The type to be returned by the factory.</param>
        public static void Register(this IFactoryService factoryService, Func<object> factoryMethod, Type type)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            factoryService.Register((IDefaultFactory<object>)typeof(DefaultFactory<>).MakeGenericType(type).CreateObject(factoryMethod), type);
        }

        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to be returned by the factory.</typeparam>
        /// <param name="factoryService">The factory service.</param>
        /// <param name="factory">The factory.</param>
        public static void Register<T>(this IFactoryService factoryService, IFactory<T> factory) where T : class => factoryService.Register(factory, typeof(T));

        /// <summary>
        /// Returns an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <param name="factoryService">The factory service.</param>
        /// <returns>An instance of an object of type T.</returns>
        public static T Get<T>(this IFactoryService factoryService) where T : class => (T)factoryService.Get(typeof(T));

        /// <summary>
        /// Returns an object of the specified type.
        /// </summary>
        /// <param name="factoryService">The factory service.</param>
        /// <param name="type">The type of the object to be returned.</param>
        /// <returns>An instance of an object of the specified type.</returns>
        public static object Get(this IFactoryService factoryService, Type type)
        {
            var factory = factoryService.GetFactory(type);
            if (factory == null)
                throw new InvalidOperationException($"No factory has been registered for {type.FullName}.");

            return factory.Get();
        }

        /// <summary>
        /// Returns the registered factory for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <returns>An instance of an object of the specified type.</returns>
        public static IFactory<T>? GetFactory<T>(this IFactoryService factoryService) where T : class => (IFactory<T>)factoryService.GetFactory(typeof(T))!;
    }
}
