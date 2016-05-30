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
        /// Adds a task to the runner queue.
        /// </summary>
        /// <param name="task">The task to be added to the queue.</param>
        void Enqueue(IEvent args);
    }
}
