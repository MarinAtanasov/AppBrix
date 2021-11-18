// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System.Net.Http;

namespace AppBrix.Web.Client.Impl;

internal sealed class HttpClientFactory : IHttpClientFactory, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
    }

    public void Uninitialize()
    {
        this.app = null;
    }
    #endregion

    #region IHttpClientFactory implementation
    public HttpClient CreateClient(string name) => this.app.Get<HttpClient>();
    #endregion

    #region Private fields and constants
    #nullable disable
    private IApp app;
    #nullable restore
    #endregion
}
