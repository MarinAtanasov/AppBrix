// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.InMemory;
using AppBrix.Data.Migrations;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Data.Tests;

public sealed class InMemoryDataTests : TestsBase<InMemoryDataModule, MigrationsDataModule>
{
    #region Setup and cleanup
    public InMemoryDataTests()
    {
        this.App.ConfigService.GetInMemoryDataConfig().ConnectionString = Guid.NewGuid().ToString();
        this.App.ConfigService.GetMigrationsDataConfig().EntryAssembly = this.GetType().Assembly.FullName!;
        this.App.Start();
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestCrudOperations()
    {
        using (var context = this.App.GetDbContextService().Get<DataItemContextMock>())
        {
            context.Items.Add(new DataItemMock { Content = nameof(InMemoryDataTests.TestCrudOperations) });
            context.SaveChanges();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemContextMock>())
        {
            var item = context.Items.Single();
            item.Id.Should().NotBe(Guid.Empty, "Id should be automatically generated");
            item.Content.Should().Be(nameof(InMemoryDataTests.TestCrudOperations), $"{nameof(item.Content)} should be saved");
            item.Content = nameof(DataItemContextMock);
            context.SaveChanges();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemContextMock>())
        {
            var item = context.Items.Single();
            item.Content.Should().Be(nameof(DataItemContextMock), $"{nameof(item.Content)} should be updated");
            context.Items.Remove(item);
            context.SaveChanges();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemContextMock>())
        {
            context.Items.Count().Should().Be(0, "the item should have been deleted");
        }
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetItem() => this.AssertPerformance(this.TestPerformanceGetItemInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetItemInternal()
    {
        using (var context = this.App.GetDbContextService().Get<DataItemContextMock>())
        {
            context.Items.Add(new DataItemMock { Content = nameof(this.TestCrudOperations) });
            context.SaveChanges();
        }

        for (var i = 0; i < 60; i++)
        {
            using var context = this.App.GetDbContextService().Get<DataItemContextMock>();
            _ = context.Items.Single();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemContextMock>())
        {
            context.Items.Remove(context.Items.Single());
            context.SaveChanges();
        }
    }
    #endregion
}
