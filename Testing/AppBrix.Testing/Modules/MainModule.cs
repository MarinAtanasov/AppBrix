// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix.Testing.Modules;

/// <summary>
/// A main module that registers the provided module type and its dependencies.
/// </summary>
/// <typeparam name="T">The type of the module being tested.</typeparam>
public sealed class MainModule<T> : MainModuleBase
	where T : class, IModule
{
	/// <summary>
	/// Gets the types of the modules which are direct dependencies for the current module.
	/// This is used to determine the order in which the modules are loaded.
	/// </summary>
	public override IEnumerable<Type> Dependencies => [typeof(T)];
}

/// <summary>
/// A main module that registers the provided module types and their dependencies.
/// </summary>
/// <typeparam name="T1">The type of the first module being tested.</typeparam>
/// <typeparam name="T2">The type of the second module being tested.</typeparam>
public sealed class MainModule<T1, T2> : MainModuleBase
	where T1 : class, IModule
	where T2 : class, IModule
{
	/// <summary>
	/// Gets the types of the modules which are direct dependencies for the current module.
	/// This is used to determine the order in which the modules are loaded.
	/// </summary>
	public override IEnumerable<Type> Dependencies => [typeof(T1), typeof(T2)];
}
