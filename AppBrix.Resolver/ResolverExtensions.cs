// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix
{
    public static class ResolverExtensions
    {
        #region IResolver extensions
        /// <summary>
        /// Gets the application's current resolver.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The registered resolver.</returns>
        public static IResolver GetResolver(this IApp app)
        {
            return ResolverExtensions.Resolvers.ContainsKey(app) ? ResolverExtensions.Resolvers[app] : null;
        }

        /// <summary>
        /// Sets the application's current resolver.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="resolver">The resolver.</param>
        public static void SetResolver(this IApp app, IResolver resolver)
        {
            lock (ResolverExtensions.Resolvers)
            {
                if (resolver != null)
                {
                    ResolverExtensions.Resolvers[app] = resolver;
                }
                else if (ResolverExtensions.Resolvers.ContainsKey(app))
                {
                    ResolverExtensions.Resolvers.Remove(app);
                }
            }
        }
        
        /// <summary>
        /// Resolves an item by its type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be resolved.</typeparam>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static T Get<T>(this IApp app) where T : class
        {
            if (typeof(T).IsAssignableFrom(app.GetType()))
            {
                return (T)app;
            }
            else if (typeof(T).IsAssignableFrom(app.ConfigManager.GetType()))
            {
                return (T)app.ConfigManager;
            }
            else
            {
                return app.GetResolver().Get<T>();
            }
        }

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
        
        #region Private fields and constants
        private static IDictionary<IApp, IResolver> Resolvers = new Dictionary<IApp, IResolver>();
        #endregion
    }
}
