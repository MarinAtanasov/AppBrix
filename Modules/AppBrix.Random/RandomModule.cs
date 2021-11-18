// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Factory;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Random.Impl;
using System;
using System.Collections.Generic;

namespace AppBrix.Random;

/// <summary>
/// Module which is used for generating random objects and values
/// </summary>
public sealed class RandomModule : ModuleBase
{
    #region Properties
    /// <summary>
    /// Gets the types of the modules which are direct dependencies for the current module.
    /// This is used to determine the order in which the modules are loaded.
    /// </summary>
    public override IEnumerable<Type> Dependencies => new[] { typeof(FactoryModule) };
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Initializes the module.
    /// Automatically called by <see cref="ModuleBase.Initialize"/>
    /// </summary>
    /// <param name="context">The initialization context.</param>
    protected override void Initialize(IInitializeContext context)
    {
        this.App.Container.Register(this);

        this.randomService.Initialize(context);
        this.App.Container.Register(this.randomService);

        this.App.GetFactoryService().Register(() => this.App.GetRandomService().GetRandom());
    }

    /// <summary>
    /// Uninitializes the module.
    /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
    /// </summary>
    protected override void Uninitialize()
    {
        this.randomService.Uninitialize();
    }
    #endregion

    #region Private fields and constants
    private readonly RandomService randomService = new RandomService();
    #endregion
}
