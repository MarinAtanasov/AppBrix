// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Data.Migrations;
using AppBrix.Data.Migrations.Configuration;
using AppBrix.Data.Migrations.Data;
using AppBrix.Data.Services;

namespace AppBrix;

/// <summary>
/// Extension methods for the <see cref="MigrationsDataModule"/>.
/// </summary>
public static class MigrationsDataExtensions
{
	/// <summary>
	/// Gets the <see cref="MigrationsDataConfig"/> from <see cref="IConfigService"/>.
	/// </summary>
	/// <param name="service">The configuration service.</param>
	/// <returns>The <see cref="MigrationsDataConfig"/>.</returns>
	public static MigrationsDataConfig GetMigrationsDataConfig(this IConfigService service) => (MigrationsDataConfig)service.Get(typeof(MigrationsDataConfig));

	/// <summary>
	/// Gets an instance of <see cref="MigrationsDbContext"/>.
	/// </summary>
	/// <returns>A database context.</returns>
	internal static MigrationsDbContext GetMigrationsContext(this IDbContextService service) => (MigrationsDbContext)service.Get(typeof(MigrationsDbContext));
}
