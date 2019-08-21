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
        /// Get the referenced assemblies, starting with the provided assembly.
        /// </summary>
        /// <param name="current">The current assembly.</param>
        /// <param name="recursive">If true, gets all references. If false, gets only the direct ones.</param>
        /// <returns>The referenced assemblies.</returns>
        public static IEnumerable<Assembly> GetReferencedAssemblies(this Assembly current, bool recursive)
        {
            var names = new HashSet<string> { current.GetName().FullName };
            var locations = new HashSet<string> { current.Location };
            var assemblyQueue = new List<Assembly> { current };

            for (int i = 0; i < assemblyQueue.Count; i++)
            {
                var referencedAssemblies = assemblyQueue[i].GetReferencedAssemblies();
                for (int j = 0; j < referencedAssemblies.Length; j++)
                {
                    var reference = referencedAssemblies[j];
                    if (!names.Add(reference.FullName))
                        continue;
                    
                    Assembly referencedAssembly;
                    try
                    {
                        referencedAssembly = Assembly.Load(reference);
                    }
                    catch (IOException)
                    {
                        // Ignore assemblies which cannot be found or loaded.
                        continue;
                    }

                    if (locations.Add(referencedAssembly.Location))
                    {
                        assemblyQueue.Add(referencedAssembly);
                    }
                }

                if (!recursive)
                {
                    break;
                }
            }

            return assemblyQueue;
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
        /// Constructs an object.    
        /// </summary>
        /// <typeparam name="T">The type in which the result should be casted</typeparam>
        /// <param name="type">The type of the object to be constructed.</param>
        /// <param name="args">The constructor arguments.</param>
        /// <returns>The constructed object.</returns>
        public static T CreateObject<T>(this Type type, params object[] args) => (T)type.CreateObject(args);

        /// <summary>
        /// Constructs an object.
        /// </summary>
        /// <param name="type">The type of the object to be constructed.</param>
        /// <param name="args">The constructor arguments.</param>
        /// <returns>The constructed object.</returns>
        public static object CreateObject(this Type type, params object[] args) => Activator.CreateInstance(type, args);

        /// <summary>
        /// Gets the assembly qualified name using only the assembly name without culture and version.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The assembly qualified name.</returns>
        public static string GetAssemblyQualifiedName(this Type type) => $"{type.FullName}, {type.Assembly.GetName().Name}";
        #endregion
    }
}
