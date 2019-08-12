// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Data.Migration;
using AppBrix.Data.Migration.Configuration;
using AppBrix.Data.Sqlite;
using AppBrix.Data.Sqlite.Configuration;
using AppBrix.Tests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Mocks;
using Xunit;

namespace AppBrix.Data.Tests
{
    public sealed class SqliteDataTests : IDisposable
    {
        #region Setup and cleanup
        public SqliteDataTests()
        {
            this.app = TestUtils.CreateTestApp(typeof(SqliteDataModule), typeof(MigrationDataModule));
            this.app.GetConfig<SqliteDataConfig>().ConnectionString = $"Data Source={Guid.NewGuid()}.db; Mode=Memory; Cache=Shared";
            this.app.GetConfig<MigrationDataConfig>().EntryAssembly = this.GetType().Assembly.FullName;
            this.app.GetConfig<AppConfig>().Modules.Single(x => x.Type == typeof(MigrationDataModule).GetAssemblyQualifiedName()).Status = ModuleStatus.Disabled;
            this.app.Start();

            this.globalContext = this.app.GetDbContextService().Get<MigrationContext>();
            this.globalContext.Database.OpenConnection();

            this.app.GetConfig<AppConfig>().Modules.Single(x => x.Type == typeof(MigrationDataModule).GetAssemblyQualifiedName()).Status = ModuleStatus.Enabled;
            this.app.Restart();
        }

        public void Dispose()
        {
            this.globalContext.Database.CloseConnection();
            this.globalContext.Dispose();
            this.app.Stop();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestCrudOperations()
        {
            using (var context = this.app.GetDbContextService().Get<DataItemContextMock>())
            {
                context.Items.Add(new DataItemMock { Content = nameof(TestCrudOperations) });
                context.SaveChanges();
            }

            using (var context = this.app.GetDbContextService().Get<DataItemContextMock>())
            {
                var item = context.Items.Single();
                item.Id.Should().NotBe(Guid.Empty, "Id should be automatically generated");
                item.Content.Should().Be(nameof(TestCrudOperations), $"{nameof(item.Content)} should be saved");
                item.Content = nameof(DataItemContextMock);
                context.SaveChanges();
            }

            using (var context = this.app.GetDbContextService().Get<DataItemContextMock>())
            {
                var item = context.Items.Single();
                item.Content.Should().Be(nameof(DataItemContextMock), $"{nameof(item.Content)} should be updated");
                context.Items.Remove(item);
                context.SaveChanges();
            }

            using (var context = this.app.GetDbContextService().Get<DataItemContextMock>())
            {
                context.Items.Count().Should().Be(0, "the item should have been deleted");
            }
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceGetItem()
        {
            TestUtils.TestPerformance(this.TestPerformanceGetItemInternal);
        }
        #endregion

        #region Private methods
        private void TestPerformanceGetItemInternal()
        {
            using (var context = this.app.GetDbContextService().Get<DataItemContextMock>())
            {
                context.Items.Add(new DataItemMock { Content = nameof(TestCrudOperations) });
                context.SaveChanges();
            }

            for (int i = 0; i < 60; i++)
            {
                using (var context = this.app.GetDbContextService().Get<DataItemContextMock>())
                {
                    context.Items.Single();
                } 
            }

            using (var context = this.app.GetDbContextService().Get<DataItemContextMock>())
            {
                context.Items.Remove(context.Items.Single());
                context.SaveChanges();
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        private readonly MigrationContext globalContext;
        #endregion
    }
}
