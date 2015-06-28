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
    /// The object cloner is used for creating deep copies of objects.
    /// </summary>
    public sealed class CloningModule : ModuleBase
    {
        #region Properties
        public override int LoadPriority
        {
            get
            {
                return (int)ModuleLoadPriority.Cloning;
            }
        }
        #endregion

        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Resolver.Register(this);
            this.App.Resolver.Register(this.cloner.Value);
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private fields and constants
        private Lazy<DefaultCloner> cloner = new Lazy<DefaultCloner>();
        #endregion
    }
}
