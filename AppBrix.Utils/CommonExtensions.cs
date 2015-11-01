// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Utils.Exceptions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AppBrix
{
    /// <summary>
    /// Used for storing commonly used extension methods.
    /// </summary>
    public static class CommonExtensions
    {
        #region Types and enums extensions
        /// <summary>
        /// Parses the string and converts it to an enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">The string.</param>
        /// <returns>The enumeration value matching the string.</returns>
        public static T ToEnum<T>(this string value) where T : struct
        {
            if (!typeof(T).GetTypeInfo().IsEnum)
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

            var constructor = type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x => x.GetParameters().Length == 0);
            if (constructor == null)
                throw new DefaultConstructorNotFoundException(
                    "Unable to find default constructor for type " + type.GetAssemblyQualifiedName());

            return constructor.Invoke(null);
        }

        /// <summary>
        /// Constructs an object with no parameters.
        /// </summary>
        /// <typeparam name="T">The type in which the result should be casted</typeparam>
        /// <param name="type">The type of the object to be constructed.</param>
        /// <returns>The constructed object.</returns>
        public static T CreateObject<T>(this Type type)
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
            return string.Concat(type.FullName, ", ", type.GetTypeInfo().Assembly.GetName().Name);
        }
        #endregion
        
        #region Strings and encodings
        /// <summary>
        /// Shorthand for getting the string from a whole byte array.
        /// </summary>
        /// <param name="encoding">The current encoding.</param>
        /// <param name="bytes">The byte array containing the data to be stringified.</param>
        /// <returns>The string which corresponds to the data inside the byte array.</returns>
        public static string GetString(this Encoding encoding, byte[] bytes)
        {
            return encoding.GetString(bytes, 0, bytes.Length);
        }
        #endregion
    }
}
