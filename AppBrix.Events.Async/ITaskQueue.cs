// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Async
{
    /// <summary>
    /// Defines a task queue which runs subscribed tasks asynchronously.
    /// </summary>
    internal interface ITaskQueue
    {
        /// <summary>
        /// Subscribes a handler to be called by the task queue.
        /// </summary>
        /// <param name="handler">The handler to be subscribed.</param>
        void Subscribe(Action<IEvent> handler);

        /// <summary>
        /// Unsubscribes a handler from the task queue.
        /// </summary>
        /// <param name="handler">The handler to be unsubscribed.</param>
        void Unsubscribe(Action<IEvent> handler);

        /// <summary>
        /// Adds a task to the runner queue.
        /// </summary>
        /// <param name="task">The task to be added to the queue.</param>
        void Enqueue(IEvent args);
    }
}
