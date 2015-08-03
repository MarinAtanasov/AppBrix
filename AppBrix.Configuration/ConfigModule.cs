// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Configuration
{
    public sealed class ConfigModule : ModuleBase
    {
        #region Properties
        public override int LoadPriority
        {
            get { return (int)ModuleLoadPriority.Config; }
        }
        #endregion

        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetResolver().Register(this);
            this.config = this.CreateConfig();
            this.config.Initialize(context);
            this.App.GetResolver().Register(this.config);
        }

        protected override void UninitializeModule()
        {
            this.config.Uninitialize();
            this.config = null;
        }
        #endregion

        #region Private methods
        private DefaultConfig CreateConfig()
        {
            var path = this.App.AppConfig.Properties[ConfigModule.ConfigPathProperty];
            return path != null ? new DefaultConfig(path.Value) : new DefaultConfig();
        }
        #endregion

        #region private fields and constants
        /// <summary>
        /// Gets the name of the application config property
        /// which contains the location of the module configuration file.
        /// </summary>
        public const string ConfigPathProperty = "ConfigPath";
        /// <summary>
        /// Gets the name of the application config property
        /// which contains the configuration save mode:
        /// Full, Minimal or Modified. Default is Minimal.
        /// </summary>
        public const string ConfigSaveModeProperty = "ConfigSaveMode";
        private DefaultConfig config;
        #endregion
    }
}
