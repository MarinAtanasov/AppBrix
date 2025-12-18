// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Events;
using AppBrix.Data.Services;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.InMemory.Impl;

internal sealed class InMemoryDbContextConfigurer : IDbContextConfigurer, IApplicationLifecycle
{
	public void Initialize(IInitializeContext context)
	{
		this.connectionString = context.App.ConfigService.GetInMemoryDataConfig().ConnectionString;
	}

	public void Uninitialize()
	{
		this.connectionString = string.Empty;
	}

	public void Configure(IConfigureDbContext context) => context.OptionsBuilder.UseInMemoryDatabase(this.connectionString);

	private string connectionString = string.Empty;
}
