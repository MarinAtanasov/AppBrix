// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Events;
using AppBrix.Data.InMemory;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using System;
using System.Linq;

namespace AppBrix.Data.Tests;

[TestClass]
public sealed class DbContextServiceTests : TestsBase<InMemoryDataModule>
{
    #region Test lifecycle
    protected override void Initialize() => this.App.Start();
    #endregion

    #region Tests
    [Test, Functional]
    public void TestGetNullType()
    {
        var service = this.App.GetDbContextService();
        var action = () => service.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "type should not be null");;
    }

    [Test, Functional]
    public void TestRaiseConfigureDbContextEvent()
    {
        DataItemDbContextMock eventDbContext = null;
        this.App.GetEventHub().Subscribe<IConfigureDbContext>(args =>
        {
            eventDbContext = args.Context as DataItemDbContextMock;
            this.Assert(args.Context is not null, "context should be provided");
            this.Assert(args.MigrationsAssembly is null, "migrations module is not available");
            this.Assert(args.MigrationsHistoryTable == "__EFMigrationsHistory", "migrations module is not available");
        });

        using var context = this.App.GetDbContextService().Get<DataItemDbContextMock>();
        this.Assert(context.Items.Any() == false, "no items have been created");
        this.Assert(object.ReferenceEquals(context, eventDbContext), "context in event should be the same as created context");
    }
    #endregion
}
