// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Exceptions;
using System;
using System.Linq;

namespace AppBrix
{
    public static class AppBrixExtensions
    {
        #region IApp extensions
        /// <summary>
        /// Unloads and loads the application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Restart(this IApp app)
        {
            app.Stop();
            app.Start();
        }

        /// <summary>
        /// Uninitializes and reinitializes the application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Reinitialize(this IApp app)
        {
            app.Uninitialize();
            app.Initialize();
        }
        #endregion

        #region Types and enums extensions
        /// <summary>
        /// Parses the string and converts it to an enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">The string.</param>
        /// <returns>The enumeration value matching the string.</returns>
        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type. T is: " + typeof(T).FullName);

            T result;
            Enum.TryParse<T>(value, true, out result);
            return result;
        }

        /// <summary>
        /// Constructs an object with no parameters.
        /// </summary>
        /// <param name="type">The type of the object to be constructed.</param>
        /// <returns>The constructed object.</returns>
        public static object CreateObject(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new DefaultConstructorMissingException(
                    "Unable to find constructor for type " + type.GetAssemblyQualifiedName());

            return constructor.Invoke(null);
        }

        /// <summary>
        /// Constructs an object with no parameters.
        /// </summary>
        /// <typeparam name="T">The type in which the result should be casted</typeparam>
        /// <param name="type">The type of the object to be constructed.</param>
        /// <returns>The constructed object.</returns>
        public static T CreateObject<T>(this Type type) where T : class
        {
            return (T)type.CreateObject();
        }

        /// <summary>
        /// Gets the assembly qualified name using only the assembly name without culture and version.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The assembly qualified name.</returns>
        public static string GetAssemblyQualifiedName(this Type type)
        {
            return string.Concat(type.FullName, ", ", type.Assembly.GetName().Name);
        }
        #endregion
    }
}
