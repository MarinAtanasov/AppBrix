// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.Impl;
using AppBrix.Factory;
using AppBrix.Lifecycle;
using AppBrix.Logging;
using AppBrix.Modules;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Module used for enabling database functionality.
    /// </summary>
    public sealed class DataModule : ModuleBase
    {
        #region Properties
        /// <summary>
        /// Gets the types of the modules which are direct dependencies for the current module.
        /// This is used to determine the order in which the modules are loaded.
        /// </summary>
        public override IEnumerable<Type> Dependencies => new[] { typeof(FactoryModule), typeof(LoggingModule) };
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
            this.contextService.Initialize(context);
            this.App.Container.Register(this.contextService);
            this.App.GetEventHub().Subscribe<IOnConfiguringDbContext>(this.ConfigureDbContextOptions);
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

        #region Private methods
        private void ConfigureDbContextOptions(IOnConfiguringDbContext args)
        {
            args.OptionsBuilder.UseLoggerFactory(this.App.Get<ILoggerFactory>());
        }
        #endregion

        #region Private fields and constants
        private readonly DefaultDbContextService contextService = new DefaultDbContextService();
        #endregion
    }
}
