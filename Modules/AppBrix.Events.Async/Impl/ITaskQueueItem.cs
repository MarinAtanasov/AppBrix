// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl;

/// <summary>
/// Defines a task queue item which wraps an event handler.
/// </summary>
internal interface ITaskQueueItem<in T> where T : IEvent
{
	/// <summary>
	/// Executes the wrapped event handler.
	/// </summary>
	/// <param name="args">The event arguments.</param>
	/// <returns>The task that will be awaited while executing the handler.</returns>
	Task Execute(T args);
}
