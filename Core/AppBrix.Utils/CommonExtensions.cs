// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.IO;
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
        /// Get all of the referenced assemblies recursively.
        /// </summary>
        /// <param name="current">The current assembly.</param>
        /// <returns>All referenced assemblies.</returns>
        public static IEnumerable<Assembly> GetAllReferencedAssemblies(this Assembly current)
        {
            var names = new HashSet<string> { current.GetName().FullName };
            var locations = new HashSet<string> { current.Location };
            var assemblyQueue = new Queue<Assembly>();
            assemblyQueue.Enqueue(current);

            while (assemblyQueue.Count > 0)
            {
                var assembly = assemblyQueue.Dequeue();
                foreach (var reference in assembly.GetReferencedAssemblies())
                {
                    if (names.Contains(reference.FullName))
                        continue;

                    names.Add(reference.FullName);

                    Assembly referencedAssembly;
                    try
                    {
                        referencedAssembly = Assembly.Load(reference);
                    }
                    catch (FileLoadException)
                    {
                        // Ignore assemblies which cannot be found or loaded.
                        continue;
                    }

                    if (locations.Contains(referencedAssembly.Location))
                        continue;

                    locations.Add(referencedAssembly.Location);
                    assemblyQueue.Enqueue(referencedAssembly);
                    yield return referencedAssembly;
                }
            }
        }

        /// <summary>
        /// Parses the string and converts it to an enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">The string.</param>
        /// <returns>The enumeration value matching the string.</returns>
        public static T ToEnum<T>(this string value) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"{nameof(T)} must be an enumerated type. {nameof(T)} is: {typeof(T).GetAssemblyQualifiedName()}");

            Enum.TryParse(value, true, out T result);
            return result;
        }

        /// <summary>
        /// Constructs an object with no parameters.
        /// </summary>
        /// <param name="type">The type of the object to be constructed.</param>
        /// <returns>The constructed object.</returns>
        public static object CreateObject(this Type type)
        {
            return Activator.CreateInstance(type);
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
            return string.Concat(type.FullName, ", ", type.Assembly.GetName().Name);
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
