// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Server.Events;
using Microsoft.AspNetCore.Builder;

namespace AppBrix.Web.Server.Impl
{
    internal sealed class ConfigureWebApp : IConfigureWebApp
    {
        #region Construction
        public ConfigureWebApp(WebApplication app)
        {
            this.App = app;
        }
        #endregion

        #region Properties
        public WebApplication App { get; }
        #endregion
    }
}
