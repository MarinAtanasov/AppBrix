// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.InMemory;
using AppBrix.Data.Migrations;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using Xunit;

namespace AppBrix.Data.Tests;

public sealed class MigrationsDbContextServiceTests : TestsBase<InMemoryDataModule, MigrationsDataModule>
{
    #region Setup and cleanup
    public MigrationsDbContextServiceTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullType()
    {
        var service = this.App.GetDbContextService();
        var action = () => service.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "type should not be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestCreateExternalDbContext()
    {
        using var context = this.App.GetDbContextService().Get<ExternalDbContextMock>();
        this.Assert(context is not null, "the db context service should return a new instance");
    }
    #endregion
}
