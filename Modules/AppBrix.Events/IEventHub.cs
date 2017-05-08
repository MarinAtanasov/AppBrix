// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events
{
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
        /// <exception cref="ArgumentNullException">handler</exception>
        void Subscribe<T>(Action<T> handler) where T : IEvent;

        /// <summary>
        /// Unsubscribes the event handler from the event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="handler">The event handler.</param>
        /// <exception cref="ArgumentNullException">handler</exception>
        /// <exception cref="ArgumentException">Handler is not registered.</exception>
        void Unsubscribe<T>(Action<T> handler) where T : IEvent;

        /// <summary>
        /// Raises the event and all of its base class and interface events.
        /// </summary>
        /// <typeparam name="T">The type of the event to be raised.</typeparam>
        /// <param name="args">The event arguments.</param>
        /// <exception cref="ArgumentNullException">args</exception>
        void Raise<T>(T args) where T : IEvent;
    }
}
