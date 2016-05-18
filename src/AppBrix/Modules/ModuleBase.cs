// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Modules
{
    /// <summary>
    /// Base class for application modules.
    /// This class will set <see cref="App"/> before calling the overriden methods.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        #region Properties
        /// <summary>
        /// Gets the current module's app.
        /// </summary>
        protected IApp App { get; private set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes the common module logic and calls the implemented InitializeModule method.
        /// </summary>
        /// <param name="context">The current initialization context.</param>
        public void Initialize(IInitializeContext context)
        {
            this.App = context.App;
            this.InitializeModule(context);
        }

        /// <summary>
        /// Calls the implemented UninitializeModule method and uninitializes the common module logic.
        /// </summary>
        public void Uninitialize()
        {
            this.UninitializeModule();
            this.App = null;
        }

        /// <summary>
        /// Called after <see cref="ModuleBase"/>'s <see cref="Initialize(IInitializeContext)"/>.
        /// </summary>
        /// <param name="context"></param>
        protected abstract void InitializeModule(IInitializeContext context);

        /// <summary>
        /// Called after <see cref="ModuleBase"/>'s <see cref="Uninitialize"/>
        /// </summary>
        protected abstract void UninitializeModule();
        #endregion
    }
}
