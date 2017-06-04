// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Data.SqlServer.Configuration;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data.SqlServer
{
    internal sealed class SqlServerDbContextConfigurer : IDbContextConfigurer, IApplicationLifecycle
    {
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.app = null;
        }

        public void Configure(IOnConfiguringDbContext context)
        {
            context.OptionsBuilder.UseSqlServer(
                this.app.GetConfig<SqlServerDataConfig>().ConnectionString,
                builder => builder.MigrationsAssembly(context.MigrationsAssembly));
        }

        private IApp app;
    }
}
