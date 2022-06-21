// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Events;
using AppBrix.Data.InMemory;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Data.Tests;

public sealed class DbContextServiceTests : TestsBase
{
    #region Setup and cleanup

    public DbContextServiceTests() : base(TestUtils.CreateTestApp<InMemoryDataModule>()) => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullType()
    {
        var service = this.app.GetDbContextService();
        var action = () => service.Get(null);
        action.Should().Throw<ArgumentNullException>("type should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRaiseConfigureDbContextEvent()
    {
        DataItemContextMock eventContext = null;;
        this.app.GetEventHub().Subscribe<IConfigureDbContext>(args =>
        {
            eventContext = args.Context as DataItemContextMock;
            args.Context.Should().NotBeNull("context should be provided");
            args.MigrationsAssembly.Should().BeNull("migrations module is not available");
            args.MigrationsHistoryTable.Should().Be("__EFMigrationsHistory", "migrations module is not available");
        });

        using var context = this.app.GetDbContextService().Get<DataItemContextMock>();
        context.Items.Count().Should().Be(0, "no items have been created");
        context.Should().BeSameAs(eventContext, "context in event should be the same as created context");
    }
    #endregion
}
