// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl;

/// <summary>
/// Defines a task queue which runs subscribed tasks asynchronously.
/// </summary>
internal interface ITaskQueue : IDisposable
{
}

/// <summary>
/// Defines a task queue which runs subscribed tasks asynchronously.
/// </summary>
internal interface ITaskQueue<T> : ITaskQueue where T : IEvent
{
	/// <summary>
	/// Gets how many handlers are subscribed in the queue.
	/// </summary>
	int Count { get; }

	/// <summary>
	/// Subscribes a synchronous event handler to be called when a new task is enqueued.
	/// </summary>
	/// <param name="handler">The handler.</param>
	void Subscribe(Action<T> handler);

	/// <summary>
	/// Subscribes an asynchronous event handler to be called when a new task is enqueued.
	/// </summary>
	/// <param name="handler">The handler.</param>
	void Subscribe(Func<T, Task> handler);

	/// <summary>
	/// Unsubscribes the event handler so that it is no longer called when a new task is enqueued.
	/// </summary>
	/// <param name="handler">The handler.</param>
	void Unsubscribe(Action<T> handler);

	/// <summary>
	/// Unsubscribes the event handler so that it is no longer called when a new task is enqueued.
	/// </summary>
	/// <param name="handler">The handler.</param>
	void Unsubscribe(Func<T, Task> handler);

	/// <summary>
	/// Adds a task to the runner queue.
	/// </summary>
	/// <param name="task">The task to be added to the queue.</param>
	void Enqueue(T task);
}
