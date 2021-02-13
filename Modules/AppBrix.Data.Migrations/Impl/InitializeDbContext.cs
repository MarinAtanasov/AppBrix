// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Data.Migrations.Impl
{
    internal sealed class InitializeDbContext : IInitializeDbContext
    {
        #region Construction
        public InitializeDbContext(IApp app, string migrationsAssembly, string migrationsHistoryTable)
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));
            if (string.IsNullOrEmpty(migrationsAssembly))
                throw new ArgumentNullException(nameof(migrationsAssembly));
            if (string.IsNullOrEmpty(migrationsHistoryTable))
                throw new ArgumentNullException(nameof(migrationsHistoryTable));

            this.App = app;
            this.MigrationsAssembly = migrationsAssembly;
            this.MigrationsHistoryTable = migrationsHistoryTable;
        }
        #endregion

        #region Propreties
        public IApp App { get; }

        public string MigrationsAssembly { get; }

        public string MigrationsHistoryTable { get; }
        #endregion
    }
}
