// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AppBrix;

/// <summary>
/// Used for storing commonly used type extension methods.
/// </summary>
public static class TypeExtensions
{
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

		for (var i = 0; i < assemblyQueue.Count; i++)
		{
			foreach (var reference in assemblyQueue[i].GetReferencedAssemblies())
			{
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
	/// Finds all non-abstract classes that implement a provided type.
	/// </summary>
	/// <param name="assemblies">The assemblies to scan.</param>
	/// <typeparam name="T">The type to be implemented.</typeparam>
	/// <returns>The non-abstract classes thaht implement the provided type.</returns>
	public static IEnumerable<Type> SelectTypes<T>(this IEnumerable<Assembly> assemblies) => assemblies
		.SelectMany(assembly => { try { return assembly.ExportedTypes; } catch (TypeLoadException) { return []; } })
		.Where(type => type is { IsClass: true, IsAbstract: false } && typeof(T).IsAssignableFrom(type));

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
	public static object CreateObject(this Type type, params object[] args) => Activator.CreateInstance(type, args)!;

	/// <summary>
	/// Gets the assembly qualified name using only the assembly name without culture and version.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>The assembly qualified name.</returns>
	public static string GetAssemblyQualifiedName(this Type type) => $"{type.FullName}, {type.Assembly.GetName().Name}";
}
