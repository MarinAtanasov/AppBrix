// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Cloning;
using System;
using System.Linq;

namespace AppBrix
{
    public static class CloningExtensions
    {
        /// <summary>
        /// Gets the registered <see cref="ICloner"/>.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The registered <see cref="ICloner"/>.</returns>
        public static ICloner GetCloner(this IApp app)
        {
            return (ICloner)app.Get(typeof(ICloner));
        }

        /// <summary>
        /// Creates a deep copy of the specified object.
        /// This method has lower performance than calling <see cref="ICloner.DeepCopy(object)"/> and casting the result.
        /// </summary>
        /// <param name="cloner">The cloner.</param>
        /// <param name="obj">The object to be copied.</param>
        /// <returns>A deep copy of the specified object.</returns>
        public static T DeepCopy<T>(this ICloner cloner, T obj)
        {
            return (T)cloner.DeepCopy(obj);
        }

        /// <summary>
        /// Creates a shallow copy of the specified object.
        /// This method has lower performance than calling <see cref="ICloner.ShallowCopy(object)"/> and casting the result.
        /// </summary>
        /// <param name="cloner">The cloner.</param>
        /// <param name="obj">The object to be copied.</param>
        /// <returns>A shallow copy of the specified object.</returns>
        public static T ShallowCopy<T>(this ICloner cloner, T obj)
        {
            return (T)cloner.ShallowCopy(obj);
        }
    }
}
