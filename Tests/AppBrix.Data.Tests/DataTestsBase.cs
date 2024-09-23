// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Migrations;
using AppBrix.Data.Migrations.Data;
using AppBrix.Data.Migrations.Events;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Modules;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Data.Tests;

public abstract class DataTestsBase<T> : TestsBase<T, MigrationsDataModule>
    where T : class, IModule
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestCrudOperations()
    {
        var migrationContext = false;
        var migratedContext = false;
        this.App.GetEventHub().Subscribe<IDbContextMigratedEvent>(args =>
        {
            if (args.Type == typeof(MigrationsDbContext))
            {
                args.PreviousVersion.Should().Be(new Version(), "No previous migration context version should exist");
                args.Version.Should().Be(typeof(MigrationsDbContext).Assembly.GetName().Version, "Migration context version should match assembly version");
                args.Type.Should().Be(typeof(MigrationsDbContext), "Migration context type should match requested type");
                migrationContext.Should().BeFalse("Migration context migration should be done only once");
                migratedContext.Should().BeFalse("Context should be called after migration context");
                migrationContext = true;
            }
            else
            {
                args.PreviousVersion.Should().Be(new Version(), "No previous context version should exist");
                args.Version.Should().Be(typeof(DataItemDbContextMock).Assembly.GetName().Version, "Context version should match assembly version");
                args.Type.Should().Be(typeof(DataItemDbContextMock), "Context type should match requested type");
                migrationContext.Should().BeTrue("Migration context should be called first");
                migratedContext.Should().BeFalse("Context migration should be done only once");
                migratedContext = true;
            }
        });

        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            context.Items.Add(new DataItemMock { Content = nameof(this.TestCrudOperations) });
            context.SaveChanges();
        }

        migrationContext.Should().BeTrue("Migration context should have been migrated");
        migratedContext.Should().BeTrue("Context should have been migrated");

        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            var item = context.Items.Single();
            item.Id.Should().NotBe(Guid.Empty, "Id should be automatically generated");
            item.Content.Should().Be(nameof(this.TestCrudOperations), $"{nameof(item.Content)} should be saved");
            item.Content = nameof(DataItemDbContextMock);
            context.SaveChanges();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            var item = context.Items.Single();
            item.Content.Should().Be(nameof(DataItemDbContextMock), $"{nameof(item.Content)} should be updated");
            context.Items.Remove(item);
            context.SaveChanges();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            context.Items.Count().Should().Be(0, "the item should have been deleted");
        }
    }
    #endregion
}
