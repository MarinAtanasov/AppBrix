// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Events;
using AppBrix.Data.InMemory;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Data.Tests;

public sealed class DbContextServiceTests : TestsBase<InMemoryDataModule>
{
    #region Setup and cleanup
    public DbContextServiceTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullType()
    {
        var service = this.App.GetDbContextService();
        var action = () => service.Get(null!);
        action.Should().Throw<ArgumentNullException>("type should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRaiseConfigureDbContextEvent()
    {
        DataItemDbContextMock eventDbContext = null;
        this.App.GetEventHub().Subscribe<IConfigureDbContext>(args =>
        {
            eventDbContext = args.Context as DataItemDbContextMock;
            args.Context.Should().NotBeNull("context should be provided");
            args.MigrationsAssembly.Should().BeNull("migrations module is not available");
            args.MigrationsHistoryTable.Should().Be("__EFMigrationsHistory", "migrations module is not available");
        });

        using var context = this.App.GetDbContextService().Get<DataItemDbContextMock>();
        context.Items.Count().Should().Be(0, "no items have been created");
        context.Should().BeSameAs(eventDbContext, "context in event should be the same as created context");
    }
    #endregion
}
