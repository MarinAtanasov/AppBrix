// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Cloning.Impl;
using AppBrix.Container;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix.Cloning;

/// <summary>
/// A module used for registering a default object cloner.
/// The object cloner is used for creating deep and shallow copies of objects.
/// </summary>
public sealed class CloningModule : ModuleBase
{
	#region Properties
	/// <summary>
	/// Gets the types of the modules which are direct dependencies for the current module.
	/// This is used to determine the order in which the modules are loaded.
	/// </summary>
	public override IEnumerable<Type> Dependencies => [typeof(ContainerModule)];
	#endregion

	#region Public and overriden methods
	/// <summary>
	/// Initializes the module.
	/// Automatically called by <see cref="ModuleBase.Initialize"/>
	/// </summary>
	/// <param name="context">The initialization context.</param>
	protected override void Initialize(IInitializeContext context)
	{
		this.App.Container.Register(this.cloningService);
	}
	#endregion

	#region Private fields and constants
	private readonly CloningService cloningService = new CloningService();
	#endregion
}
