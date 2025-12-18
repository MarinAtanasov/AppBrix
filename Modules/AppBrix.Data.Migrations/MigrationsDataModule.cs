// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Migrations.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix.Data.Migrations;

/// <summary>
/// Module used for enabling database CodeFirst migration functionality.
/// </summary>
public sealed class MigrationsDataModule : ModuleBase
{
	#region Properties
	/// <summary>
	/// Gets the types of the modules which are direct dependencies for the current module.
	/// This is used to determine the order in which the modules are loaded.
	/// </summary>
	public override IEnumerable<Type> Dependencies => [typeof(DataModule)];
	#endregion

	#region Public and overriden methods
	/// <summary>
	/// Initializes the module.
	/// Automatically called by <see cref="ModuleBase.Initialize"/>
	/// </summary>
	/// <param name="context">The initialization context.</param>
	protected override void Initialize(IInitializeContext context)
	{
		this.contextService.Initialize(context);
		this.App.Container.Register(this.contextService);
	}

	/// <summary>
	/// Uninitializes the module.
	/// Automatically called by <see cref="ModuleBase.Uninitialize"/>
	/// </summary>
	protected override void Uninitialize()
	{
		this.contextService.Uninitialize();
	}
	#endregion

	#region Private fields and constants
	private readonly MigrationsDbContextService contextService = new MigrationsDbContextService();
	#endregion
}
