// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Container;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Text.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBrix.Text;

/// <summary>
/// A module which is used for enhanced text and encoding support.
/// </summary>
public sealed class TextModule : ModuleBase
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
        this.App.Container.Register(this);
        this.App.Container.Register(this.stringDistanceService);
        this.App.Container.Register(CodePagesEncodingProvider.Instance);

        this.wrapper.Initialize(context);
        Encoding.RegisterProvider(this.wrapper);
    }

    /// <summary>
    /// Uninitializes the module.
    /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
    /// </summary>
    protected override void Uninitialize()
    {
        this.wrapper.Uninitialize();
    }
    #endregion

    #region Private fields and constants
    private readonly StringDistanceService stringDistanceService = new StringDistanceService();
    private readonly EncodingProviderWrapper wrapper = new EncodingProviderWrapper();
    #endregion
}
