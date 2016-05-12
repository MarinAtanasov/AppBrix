// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Cloning
{
    /// <summary>
    /// A module used for registering a default object cloner.
    /// The object cloner is used for creating deep and shallow copies of objects.
    /// </summary>
    public sealed class CloningModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetResolver().Register(this);
            this.App.GetResolver().Register(this.cloner.Value);
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultCloner> cloner = new Lazy<DefaultCloner>();
        #endregion
    }
}
