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
    /// Base interface for application modules.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        #region Properties
        public virtual int LoadPriority
        {
            get { return (int)ModuleLoadPriority.Default; }
        }

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

        protected abstract void InitializeModule(IInitializeContext context);

        protected abstract void UninitializeModule();
        #endregion
    }
}
