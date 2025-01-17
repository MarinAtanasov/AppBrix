// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.InMemory;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using System;
using System.Linq;

namespace AppBrix.Data.Tests;

[TestClass]
public sealed class InMemoryDataTests : DataTestsBase<InMemoryDataModule>
{
    #region Test lifecycle
    protected override void Initialize()
    {
        this.App.ConfigService.GetInMemoryDataConfig().ConnectionString = Guid.NewGuid().ToString();
        this.App.Start();
    }
    #endregion

    #region Tests
    [Test, Performance]
    public void TestPerformanceGetItem() => this.AssertPerformance(this.TestPerformanceGetItemInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetItemInternal()
    {
        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            context.Items.Add(new DataItemMock { Content = nameof(this.TestCrudOperations) });
            context.SaveChanges();
        }

        for (var i = 0; i < 60; i++)
        {
            using var context = this.App.GetDbContextService().Get<DataItemDbContextMock>();
            _ = context.Items.Single();
        }

        using (var context = this.App.GetDbContextService().Get<DataItemDbContextMock>())
        {
            context.Items.Remove(context.Items.Single());
            context.SaveChanges();
        }
    }
    #endregion
}
