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
using System.Collections.Generic;
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
        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            context.Items.Add(new DataItemMock { Content = nameof(this.TestCrudOperations) });
            context.SaveChanges();
        }

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

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestMigrationEvents()
    {
        var started = new HashSet<Type>();
        var completed = new HashSet<Type>();
        
        this.App.GetEventHub().Subscribe<IDbContextMigratingEvent>(args =>
        {
            this.VerifyMigrationContextData(args);
            started.Add(args.Type).Should().BeTrue($"{args.Type.Name} migration should be started only once");
            completed.Should().NotContain(args.Type, $"{args.Type.Name} migration should not be completed before being started");
        });
        this.App.GetEventHub().Subscribe<IDbContextMigratedEvent>(args =>
        {
            this.VerifyMigrationContextData(args);
            started.Should().Contain(args.Type, $"{args.Type.Name} migration should be started before being completed");
            completed.Add(args.Type).Should().BeTrue($"{args.Type.Name} migration should be completed only once");
        });
        this.App.GetEventHub().Subscribe<IDbContextMigrationEvent>(args =>
        {
            this.VerifyMigrationContextData(args);
            if (args.Type == typeof(MigrationsDbContext))
                started.Should().NotContain(typeof(DataItemDbContextMock), "Context migration should be after migration context");
            else
                completed.Should().Contain(typeof(MigrationsDbContext), "Migration context should be before context migration");
        });

        using (this.App.GetDbContextService().Get<DataItemDbContextMock>()) { }

        completed.Should().HaveCount(2, "Both contexts should have been migrated");

        using (this.App.GetDbContextService().Get<DataItemDbContextMock>()) { }
    }
    #endregion

    #region Private methods
    private void VerifyMigrationContextData(IDbContextMigrationEvent args)
    {
        var type = args.Type == typeof(MigrationsDbContext) ? typeof(MigrationsDbContext) : typeof(DataItemDbContextMock);
        args.PreviousVersion.Should().Be(new Version(), $"No previous {type.Name} version should exist");
        args.Version.Should().Be(type.Assembly.GetName().Version, $"{type.Name} version should match assembly version");
        args.Type.Should().Be(type, $"{type.Name} type should match requested type");
    }
    #endregion
}
