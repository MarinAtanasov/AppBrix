// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.InMemory;
using AppBrix.Data.Migrations;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using System;

namespace AppBrix.Data.Tests;

[TestClass]
public sealed class MigrationsDbContextServiceTests : TestsBase<InMemoryDataModule, MigrationsDataModule>
{
    #region Tests
    [Test, Functional]
    public void TestGetNullType()
    {
        var service = this.App.GetDbContextService();
        var action = () => service.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "type should not be null");;
    }

    [Test, Functional]
    public void TestCreateExternalDbContext()
    {
        using var context = this.App.GetDbContextService().Get<ExternalDbContextMock>();
        this.Assert(context is not null, "the db context service should return a new instance");
    }
    #endregion
}
