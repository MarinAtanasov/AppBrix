﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Data.Impl;

internal sealed class InitializeDbContext : IInitializeDbContext
{
    #region Construction
    public InitializeDbContext(IApp app, string? migrationsAssembly = null, string migrationsHistoryTable = "__EFMigrationsHistory")
    {
        if (app is null)
            throw new ArgumentNullException(nameof(app));

        this.App = app;
        this.MigrationsAssembly = migrationsAssembly;
        this.MigrationsHistoryTable = migrationsHistoryTable;
    }
    #endregion

    #region Propreties
    public IApp App { get; }

    public string? MigrationsAssembly { get; }

    public string MigrationsHistoryTable { get; }
    #endregion
}
