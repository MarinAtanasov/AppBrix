// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Factory.Impl;
using System;

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
        /// <typeparam name="T">The type to be returned by the factory.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<T>(Func<T> factoryMethod) where T : class
        {
            if (factoryMethod is null)
                throw new ArgumentNullException(nameof(factoryMethod));

            this.Register(factoryMethod, typeof(T));
        }

        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="type">The type to be returned by the factory.</param>
        public void Register(Func<object> factoryMethod, Type type)
        {
            if (factoryMethod is null)
                throw new ArgumentNullException(nameof(factoryMethod));
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            this.Register((IDefaultFactory<object>)typeof(DefaultFactory<>).MakeGenericType(type).CreateObject(factoryMethod), type);
        }

        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to be returned by the factory.</typeparam>
        /// <param name="factory">The factory.</param>
        public void Register<T>(IFactory<T> factory) where T : class => this.Register(factory, typeof(T));

        /// <summary>
        /// Registers a factory method for the specified type.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="type">The type to be returned by the factory.</param>
        void Register(IFactory<object> factory, Type type);

        /// <summary>
        /// Returns an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <returns>An instance of an object of type T.</returns>
        public T Get<T>() where T : class => (T)this.Get(typeof(T));

        /// <summary>
        /// Returns an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to be returned.</param>
        /// <returns>An instance of an object of the specified type.</returns>
        public object Get(Type type)
        {
            var factory = this.GetFactory(type);
            if (factory is null)
                throw new InvalidOperationException($"No factory has been registered for {type.FullName}.");

            return factory.Get();
        }

        /// <summary>
        /// Returns the registered factory for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <returns>An instance of an object of the specified type.</returns>
        public IFactory<T>? GetFactory<T>() where T : class => (IFactory<T>)this.GetFactory(typeof(T))!;

        /// <summary>
        /// Returns the registered factory for the specified type.
        /// </summary>
        /// <param name="type">The type of the object to be returned.</param>
        /// <returns>An instance of an object of the specified type.</returns>
        IFactory<object>? GetFactory(Type type);
    }
}
