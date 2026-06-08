// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Sqlite;
using AppBrix.Data.Tests.Mocks;
using AppBrix.Testing;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;

namespace AppBrix.Data.Tests;

[TestClass]
public sealed class SqliteDataTests : DataTests<SqliteDataModule>
{
	#region Test lifecycle
	protected override void Initialize()
	{
		this.connection = new SqliteConnection($"Data Source={Guid.NewGuid()}.sqlite3; Mode=Memory; Cache=Shared;");
		this.connection.Open();
		this.App.ConfigService.GetSqliteDataConfig().ConnectionString = this.connection.ConnectionString;

		base.Initialize();
	}

	protected override void Uninitialize()
	{
		base.Uninitialize();

		this.connection.Close();
		this.connection.Dispose();
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

		for (var i = 0; i < 30; i++)
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

	#region Private fields and constants
	private SqliteConnection connection;
	#endregion
}
