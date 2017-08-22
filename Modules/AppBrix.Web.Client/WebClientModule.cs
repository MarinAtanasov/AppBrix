// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using AppBrix.Lifecycle;
using AppBrix.Web.Client.Configuration;
using AppBrix.Web.Client.Impl;
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
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            this.App.GetFactory().Register(() => new HttpClientHandler());
            this.App.GetFactory().Register(() => new HttpClient(this.App.GetFactory().Get<HttpMessageHandler>(), true)
            {
                Timeout = this.App.GetConfig<WebClientConfig>().RequestTimeout
            });
            this.App.GetFactory().Register(() => new DefaultHttpRequest(this.App));
            this.client = this.App.GetFactory().Get<HttpClient>();
            this.App.GetContainer().Register(this.client);
        }

        protected override void UninitializeModule()
        {
            this.client?.Dispose();
            this.client = null;
        }
        #endregion

        #region Private fields and constants
        private HttpClient client;
        #endregion
    }
}
