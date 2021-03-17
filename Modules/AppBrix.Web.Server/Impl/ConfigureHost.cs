// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Server.Events;
using Microsoft.Extensions.Hosting;

namespace AppBrix.Web.Server.Impl
{
    internal sealed class ConfigureHost : IConfigureHost
    {
        #region Construction
        public ConfigureHost(IHostBuilder builder)
        {
            this.Builder = builder;
        }
        #endregion

        #region Properties
        public IHostBuilder Builder { get; }
        #endregion
    }
}
