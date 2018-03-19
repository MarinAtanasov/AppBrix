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
            this.App.GetFactory().Register(() => new HttpClientHandler
            {
                MaxConnectionsPerServer = config.MaxConnectionsPerServer
            });
            this.App.GetFactory().Register(() => new HttpClient(this.App.GetFactory().Get<HttpMessageHandler>(), true)
            {
                Timeout = config.RequestTimeout
            });
            this.App.GetFactory().Register(() => new DefaultHttpRequest(this.App));
            this.client = this.App.GetFactory().Get<HttpClient>();
            this.App.Container.Register(this.client);

            this.oldSettingsFactory = JsonConvert.DefaultSettings;
            this.App.GetFactory().Register(() =>
            {
                var settings = this.oldSettingsFactory();
                settings.DateFormatString = this.App.GetConfig<TimeConfig>().Format;
                return settings;
            });
            JsonConvert.DefaultSettings = () => this.App.GetFactory().Get<JsonSerializerSettings>();
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            JsonConvert.DefaultSettings = this.oldSettingsFactory;
            this.client?.Dispose();
            this.client = null;
        }
        #endregion

        #region Private fields and constants
        private HttpClient client;
        private Func<JsonSerializerSettings> oldSettingsFactory;
        #endregion
    }
}
