// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl;

internal class AsyncTaskQueueItem<T> : ITaskQueueItem<T> where T : IEvent
{
    public AsyncTaskQueueItem(Func<T, Task> handler)
    {
        this.Handler = handler;
    }

    public Func<T, Task> Handler { get; }

    public Task Execute(T args) => this.Handler(args);
}
