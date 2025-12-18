// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Tests.Mocks;

/// <summary>
/// A dummy DB context used during tests.
/// </summary>
public sealed class DataItemDbContextMock : DbContextBase
{
	/// <summary>
	/// Gets or sets the items in the context.
	/// </summary>
	public DbSet<DataItemMock> Items { get; set; }
}
