// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Web.Server.Events;
using Microsoft.AspNetCore.Builder;

namespace AppBrix.Web.Server.Impl;

internal sealed class ConfigureWebAppBuilder : IConfigureWebAppBuilder
{
    #region Construction
    public ConfigureWebAppBuilder(WebApplicationBuilder builder)
    {
        this.Builder = builder;
    }
    #endregion

    #region Properties
    public WebApplicationBuilder Builder { get; }
    #endregion
}
