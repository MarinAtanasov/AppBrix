// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory.Contracts;
using AppBrix.Factory.Services;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;

namespace AppBrix.Factory.Impl;

internal sealed class FactoryService : IFactoryService, IApplicationLifecycle
{
	#region IApplicationLifecycle implementation
	public void Initialize(IInitializeContext context)
	{
	}

	public void Uninitialize()
	{
		this.factories.Clear();
	}
	#endregion

	#region IFactory implementation
	public void Register(IFactory<object> factory, Type type)
	{
		if (factory is null)
			throw new ArgumentNullException(nameof(factory));
		if (type is null)
			throw new ArgumentNullException(nameof(type));

		for (var baseType = type; baseType != typeof(object) && baseType is not null; baseType = baseType.BaseType)
		{
			this.factories[baseType] = factory;
		}

		foreach (var typeInterface in type.GetInterfaces())
		{
			this.factories[typeInterface] = factory;
		}
	}

	public IFactory<object>? GetFactory(Type type) => this.factories.GetValueOrDefault(type);
	#endregion

	#region Private fields and constants
	private readonly Dictionary<Type, IFactory<object>> factories = new Dictionary<Type, IFactory<object>>();
	#endregion
}
