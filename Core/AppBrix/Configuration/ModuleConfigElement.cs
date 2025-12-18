// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Modules;
using System;

namespace AppBrix.Configuration;

/// <summary>
/// A configuration element holding the information about an application module.
/// </summary>
public sealed class ModuleConfigElement
{
	#region Construction
	/// <summary>
	/// Creates a new instance of <see cref="ModuleConfigElement"/>.
	/// </summary>
	public ModuleConfigElement()
	{
		this.Type = string.Empty;
	}

	/// <summary>
	/// Creates a new element for the selected module.
	/// </summary>
	/// <typeparam name="T">The type of the module.</typeparam>
	/// <returns>The module element.</returns>
	public static ModuleConfigElement Create<T>() where T : IModule => ModuleConfigElement.Create(typeof(T));

	/// <summary>
	/// Creates a new element for the selected module.
	/// </summary>
	/// <param name="type">The type of the module.</param>
	/// <returns>The module element.</returns>
	public static ModuleConfigElement Create(Type type)
	{
		if (type is null)
			throw new ArgumentNullException(nameof(type));
		if (!typeof(IModule).IsAssignableFrom(type))
			throw new ArgumentException($"Type {type} is not of type {nameof(IModule)}.");

		return new ModuleConfigElement
		{
			Type = type.GetAssemblyQualifiedName()
		};
	}
	#endregion

	#region Properties
	/// <summary>
	/// Gets or sets the type of the module.
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// Gets or sets the module's current status.
	/// Changing this property requires application restart.
	/// </summary>
	public ModuleStatus Status { get; set; }

	/// <summary>
	/// Gets or sets the module's installed version.
	/// This property is automatically updated when the module is installed or upgraded.
	/// An install can be forced by removing the value.
	/// An upgrade can be forced by lowering the value to a previous version of the assembly.
	/// </summary>
	public Version? Version { get; set; }
	#endregion
}
