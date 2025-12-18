// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Modules;

namespace AppBrix.Application;

/// <summary>
/// Used for storing a part of module and its configuration element.
/// </summary>
internal sealed class ModuleInfo
{
	#region Construction
	public ModuleInfo(IModule module, ModuleConfigElement config)
	{
		this.Module = module;
		this.Config = config;
		this.Status = config.Status;
	}
	#endregion

	#region Properties
	/// <summary>
	/// Gets the instance of the module which is used by the application.
	/// </summary>
	public IModule Module { get; }

	/// <summary>
	/// Gets the configuration element for the module.
	/// </summary>
	public ModuleConfigElement Config { get; }

	/// <summary>
	/// Gets the initial status of the module when this instance of <see cref="ModuleInfo"/> was created.
	/// </summary>
	public ModuleStatus Status { get; }
	#endregion
}
