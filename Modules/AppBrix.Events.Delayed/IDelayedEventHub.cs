// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Events.Delayed;

/// <summary>
/// Used for working with application level delayed events.
/// </summary>
public interface IDelayedEventHub : IEventHub
{
    /// <summary>
    /// Raise all delayed events.
    /// </summary>
    void Flush();

    /// <summary>
    /// Delays the event and all of its base class and interface events.
    /// Event will be executed on the next <see cref="Flush"/>.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    void RaiseDelayed(IEvent args);

    /// <summary>
    /// Immediately raises the event and all of its base class and interface events.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    void RaiseImmediate(IEvent args);
}
