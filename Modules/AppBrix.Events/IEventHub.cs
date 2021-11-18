// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Events;

/// <summary>
/// Used for working with application level events.
/// </summary>
public interface IEventHub
{
    /// <summary>
    /// Subscribes the event handler to the event.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="handler">The event handler. Required.</param>
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    /// <summary>
    /// Unsubscribes the event handler from the event.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="handler">The event handler.</param>
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;

    /// <summary>
    /// Raises the event and all of its base class and interface events.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    void Raise(IEvent args);
}
