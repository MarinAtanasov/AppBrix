// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Data.Migrations;
using AppBrix.Data.Migrations.Data;
using AppBrix.Data.Sqlite;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data.Tests;

[TestClass]
public sealed class SqliteDataTests : DataTestsBase<SqliteDataModule>
{
    #region Test lifecycle
    protected override void Initialize()
    {
        this.App.ConfigService.GetSqliteDataConfig().ConnectionString = $"Data Source={Guid.NewGuid()}.db; Mode=Memory; Cache=Shared";
        this.App.Start();

        this.App.ConfigService.GetAppConfig().Modules.Single(x => x.Type == typeof(MigrationsDataModule).GetAssemblyQualifiedName()).Status = ModuleStatus.Disabled;
        this.App.Restart();

        this.globalDbContext = this.App.GetDbContextService().Get<MigrationsDbContext>();
        this.globalDbContext.Database.OpenConnection();

        this.App.ConfigService.GetAppConfig().Modules.Single(x => x.Type == typeof(MigrationsDataModule).GetAssemblyQualifiedName()).Status = ModuleStatus.Enabled;
        this.App.Restart();
    }

    public override void Stop()
    {
        this.globalDbContext.Database.CloseConnection();
        this.globalDbContext.Dispose();
        base.Stop();
    }
    #endregion

    #region Tests
    [Test, Performance]
    public void TestPerformanceGetItem() => this.AssertPerformance(this.TestPerformanceGetItemInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetItemInternal()
    {
        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            context.Items.Add(new DataItemMock { Content = nameof(this.TestCrudOperations) });
            context.SaveChanges();
        }

        for (var i = 0; i < 30; i++)
        {
            using var context = this.App.GetDbContextService().Get<DataItemDbContextMock>();
            _ = context.Items.Single();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            context.Items.Remove(context.Items.Single());
            context.SaveChanges();
        }
    }
    #endregion

    #region Private fields and constants
    private MigrationsDbContext globalDbContext;
    #endregion
}
