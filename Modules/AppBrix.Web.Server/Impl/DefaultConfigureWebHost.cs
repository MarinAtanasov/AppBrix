// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Server.Events;
using Microsoft.AspNetCore.Hosting;

namespace AppBrix.Web.Server.Impl
{
    internal sealed class DefaultConfigureWebHost : IConfigureWebHost
    {
        #region Construction
        public DefaultConfigureWebHost(IWebHostBuilder builder)
        {
            this.Builder = builder;
        }
        #endregion

        #region Properties
        public IWebHostBuilder Builder { get; }
        #endregion
    }
}
