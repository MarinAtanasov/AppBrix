// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;

namespace AppBrix.Data.Migrations.Events;

/// <summary>
/// Event that is called when a database context event has occurred.
/// </summary>
public interface IDbContextMigrationEvent : IEvent
{
    /// <summary>
    /// Gets the previous version of the database context.
    /// </summary>
    Version PreviousVersion { get; }

    /// <summary>
    /// Gets the current version of the database context.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets the type of the database context.
    /// </summary>
    Type Type { get; }
}
