// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.InMemory.Configuration;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data.InMemory.Impl
{
    internal sealed class InMemoryDbContextConfigurer : IDbContextConfigurer, IApplicationLifecycle
    {
        public void Initialize(IInitializeContext context)
        {
            this.connectionString = context.App.GetConfig<InMemoryDataConfig>().ConnectionString;
        }

        public void Uninitialize()
        {
            this.connectionString = null;
        }

        public void Configure(IOnConfiguringDbContext context)
        {
            context.OptionsBuilder.UseInMemoryDatabase(this.connectionString);
        }

        private string connectionString;
    }
}
