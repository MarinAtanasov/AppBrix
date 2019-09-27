// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using AppBrix.Lifecycle;
using AppBrix.Time.Configuration;
using AppBrix.Web.Client.Configuration;
using AppBrix.Web.Client.Impl;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using AppBrix.Factory;
using AppBrix.Time;
using System.Collections.Generic;

namespace AppBrix.Web.Client
{
    /// <summary>
    /// Modules which registers a factory for creating <see cref="IHttpRequest"/> objects.
    /// </summary>
    public sealed class WebClientModule : ModuleBase
    {
        #region Properties
        /// <summary>
        /// Gets the types of the modules which are direct dependencies for the current module.
        /// This is used to determine the order in which the modules are loaded.
        /// </summary>
        public override IEnumerable<Type> Dependencies => new[] { typeof(FactoryModule), typeof(TimeModule) };
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

            var config = this.App.GetConfig<WebClientConfig>();
            this.client = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = config.MaxConnectionsPerServer }, true)
            {
                Timeout = config.RequestTimeout
            };
            this.App.Container.Register(this.client);

            this.httpClientFactory.Initialize(context);
            this.App.Container.Register(this.httpClientFactory);
            this.App.GetFactoryService().Register(this.CreateRequest);

            this.timeConfig = this.App.GetConfig<TimeConfig>();

            this.oldSettingsFactory = JsonConvert.DefaultSettings;
            this.App.GetFactoryService().Register(this.CreateSerializerSettings);
            this.newSettingsFactory = this.GetSerializerSettings;
            JsonConvert.DefaultSettings = this.newSettingsFactory;
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void Uninitialize()
        {
            if (JsonConvert.DefaultSettings == this.newSettingsFactory)
            {
                JsonConvert.DefaultSettings = this.oldSettingsFactory;
            }

            this.httpClientFactory.Uninitialize();
            this.timeConfig = null;
            this.client?.Dispose();
            this.client = null;
        }
        #endregion

        #region Private methods
        private DefaultHttpRequest CreateRequest() => new DefaultHttpRequest(this.App);

        private JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = this.oldSettingsFactory != null ? this.oldSettingsFactory() : new JsonSerializerSettings();
            settings.DateFormatString = this.timeConfig.Format;
            return settings;
        }

        private JsonSerializerSettings GetSerializerSettings() => this.App.GetFactoryService().Get<JsonSerializerSettings>();
        #endregion

        #region Private fields and constants
        private readonly DefaultHttpClientFactory httpClientFactory = new DefaultHttpClientFactory();
        private HttpClient? client;
        #nullable disable
        private TimeConfig timeConfig;
        private Func<JsonSerializerSettings> oldSettingsFactory;
        private Func<JsonSerializerSettings> newSettingsFactory;
        #nullable restore
        #endregion
    }
}
