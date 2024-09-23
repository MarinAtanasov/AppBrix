// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

namespace AppBrix.Data.Migrations.Events;

/// <summary>
/// Event that is called when a database context has been migrated to a newer version.
/// </summary>
public interface IDbContextMigratedEvent : IDbContextMigrationEvent;
