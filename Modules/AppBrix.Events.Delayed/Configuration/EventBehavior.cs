// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Events.Delayed.Configuration;

/// <summary>
/// Enumeration used for configuring default event behavior.
/// </summary>
public enum EventBehavior
{
    /// <summary>
    /// Events will be raised immediately by default.
    /// </summary>
    Immediate,

    /// <summary>
    /// Events will be delayed by default and will be executed on the next <see cref="IDelayedEventHub.Flush"/>.
    /// </summary>
    Delayed
}
