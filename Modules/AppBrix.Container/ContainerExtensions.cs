// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix application service containers.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Resolves an item by its type.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="type">The type of the object to be resolved.</param>
        /// <returns></returns>
        public static object Get(this IApp app, Type type) => app.Container.Get(type);

        /// <summary>
        /// Resolves an item by its type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be resolved.</typeparam>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static T Get<T>(this IApp app) where T : class => app.Container.Get<T>();
    }
}
