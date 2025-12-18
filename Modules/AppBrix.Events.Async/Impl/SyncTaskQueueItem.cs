// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl;

internal class SyncTaskQueueItem<T> : ITaskQueueItem<T> where T : IEvent
{
	public SyncTaskQueueItem(Action<T> handler)
	{
		this.Handler = handler;
	}

	public Action<T> Handler { get; }

	public Task Execute(T args)
	{
		this.Handler(args);
		return Task.CompletedTask;
	}
}
