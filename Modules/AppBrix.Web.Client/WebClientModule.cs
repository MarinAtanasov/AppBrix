﻿// Copyright (c) MarinAtanasov. All rights reserved.
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

namespace AppBrix.Web.Client
{
    /// <summary>
    /// Modules which registers a factory for creating <see cref="IHttpRequest"/> objects.
    /// </summary>
    public sealed class WebClientModule : ModuleBase
    {
        #region Public and overriden methods
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            var config = this.App.GetConfig<WebClientConfig>();
            this.client = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = config.MaxConnectionsPerServer }, true)
            {
                Timeout = config.RequestTimeout
            };

            this.App.Container.Register(this.client);
            this.App.GetFactory().Register(this.App.Get<HttpClient>);
            this.App.GetFactory().Register(() => new DefaultHttpRequest(this.App));

            this.oldSettingsFactory = JsonConvert.DefaultSettings;
            this.App.GetFactory().Register(() =>
            {
                var settings = this.oldSettingsFactory != null ? this.oldSettingsFactory() : new JsonSerializerSettings();
                settings.DateFormatString = this.App.GetConfig<TimeConfig>().Format;
                return settings;
            });
            this.newSettingsFactory = () => this.App.GetFactory().Get<JsonSerializerSettings>();
            JsonConvert.DefaultSettings = this.newSettingsFactory;
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            if (JsonConvert.DefaultSettings == this.newSettingsFactory)
            {
                JsonConvert.DefaultSettings = this.oldSettingsFactory;
            }
            this.client?.Dispose();
            this.client = null;
        }
        #endregion

        #region Private fields and constants
        private HttpClient client;
        private Func<JsonSerializerSettings> oldSettingsFactory;
        private Func<JsonSerializerSettings> newSettingsFactory;
        #endregion
    }
}
