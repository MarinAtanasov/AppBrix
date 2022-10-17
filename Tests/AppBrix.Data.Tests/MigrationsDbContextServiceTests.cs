// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.InMemory;
using AppBrix.Data.Migrations;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Data.Tests;

public sealed class MigrationsDbContextServiceTests : TestsBase<InMemoryDataModule, MigrationsDataModule>
{
    #region Setup and cleanup
    public MigrationsDbContextServiceTests() => this.app.Start();
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
    public void TestCreateExternalDbContext()
    {
        using var context = this.app.GetDbContextService().Get<ExternalDbContextMock>();
        context.Should().NotBeNull("the db context service should return a new instance");
    }
    #endregion
}
