// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Data.Migration.Impl
{
    internal sealed class DefaultInitializeDbContext : IInitializeDbContext
    {
        #region Construction
        public DefaultInitializeDbContext(IApp app, string migrationsAssembly)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (string.IsNullOrEmpty(migrationsAssembly))
                throw new ArgumentNullException(nameof(migrationsAssembly));

            this.App = app;
            this.MigrationsAssembly = migrationsAssembly;
        }
        #endregion

        #region Propreties
        public IApp App { get; }

        public string MigrationsAssembly { get; }
        #endregion
    }
}
