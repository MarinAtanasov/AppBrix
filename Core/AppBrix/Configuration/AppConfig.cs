// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System.Collections.Generic;

namespace AppBrix.Configuration;

/// <summary>
/// Configuration which holds the modules collection.
/// </summary>
public sealed class AppConfig : IConfig
{
	#region Construction
	/// <summary>
	/// Creates a new instance of <see cref="AppConfig"/>.
	/// </summary>
	public AppConfig()
	{
		this.Modules = new List<ModuleConfigElement>();
	}
	#endregion

	#region Properties
	/// <summary>
	/// Gets a list of modules to be loaded by the application.
	/// </summary>
	public List<ModuleConfigElement> Modules { get; private set; }
	#endregion
}
