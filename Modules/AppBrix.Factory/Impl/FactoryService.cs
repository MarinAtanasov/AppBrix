// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory.Contracts;
using AppBrix.Factory.Services;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;

namespace AppBrix.Factory.Impl;

/// <summary>
/// Default factory which will execute the default constructor
/// unless a different method has been registered.
/// </summary>
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

        var baseType = type;
        while (baseType != typeof(object) && baseType is not null)
        {
            this.factories[baseType] = factory;
            baseType = baseType.BaseType;
        }

        var interfaces = type.GetInterfaces();
        for (var i = 0; i < interfaces.Length; i++)
        {
            this.factories[interfaces[i]] = factory;
        }
    }

    public IFactory<object>? GetFactory(Type type) => this.factories.TryGetValue(type, out var factory) ? factory : null;
    #endregion

    #region Private fields and constants
    private readonly Dictionary<Type, IFactory<object>> factories = new Dictionary<Type, IFactory<object>>();
    #endregion
}
