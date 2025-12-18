// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Data.Migrations.Events;

internal class DbContextMigratedEvent : IDbContextMigratedEvent
{
	public DbContextMigratedEvent(Version previousVersion, Version version, Type type)
	{
		this.PreviousVersion = previousVersion;
		this.Version = version;
		this.Type = type;
	}

	public Version PreviousVersion { get; }

	public Version Version { get; }

	public Type Type { get; }
}
