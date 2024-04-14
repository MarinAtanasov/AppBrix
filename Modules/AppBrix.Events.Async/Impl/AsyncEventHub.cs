// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Async.Services;
using AppBrix.Events.Contracts;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl;

internal sealed class AsyncEventHub : IAsyncEventHub, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
    }

    public void Uninitialize()
    {
        this.taskQueues.Keys.ToList().ForEach(this.RemoveTaskQueue);
        this.app = null!;
    }
    #endregion

    #region IAsyncEventHub implementation
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        if (this.taskQueues.TryGetValue(typeof(T), out var queueObject))
            ((TaskQueue<T>)queueObject).Subscribe(handler);
        else
            this.CreateTaskQueue<T>().Subscribe(handler);
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        if (this.taskQueues.TryGetValue(typeof(T), out var queueObject))
            ((TaskQueue<T>)queueObject).Subscribe(handler);
        else
            this.CreateTaskQueue<T>().Subscribe(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        if (this.taskQueues.TryGetValue(typeof(T), out var queueObject))
        {
            var queue = (TaskQueue<T>)queueObject;
            queue.Unsubscribe(handler);
            if (queue.Count == 0)
                this.RemoveTaskQueue(typeof(T));
        }
    }

    public void Unsubscribe<T>(Func<T, Task> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        if (this.taskQueues.TryGetValue(typeof(T), out var queueObject))
        {
            var queue = (TaskQueue<T>)queueObject;
            queue.Unsubscribe(handler);
            if (queue.Count == 0)
                this.RemoveTaskQueue(typeof(T));
        }
    }

    public void Raise(IEvent args)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        this.app.GetEventHub().Raise(args);
    }
    #endregion

    #region Private methods
    private TaskQueue<T> CreateTaskQueue<T>() where T : IEvent
    {
        var queue = new TaskQueue<T>();
        this.taskQueues[typeof(T)] = queue;
        this.app.GetEventHub().Subscribe<T>(this.RaiseEvent);
        this.taskQueueUnsubscribers[typeof(T)] = () => this.app.GetEventHub().Unsubscribe<T>(this.RaiseEvent);
        return queue;
    }

    private void RaiseEvent<T>(T args) where T : IEvent
    {
        if (this.taskQueues.TryGetValue(typeof(T), out var queueObject))
            ((TaskQueue<T>)queueObject).Enqueue(args);
    }

    private void RemoveTaskQueue(Type type)
    {
        this.taskQueueUnsubscribers[type].Invoke();
        this.taskQueueUnsubscribers.Remove(type);
        this.taskQueues[type].Dispose();
        this.taskQueues.Remove(type);
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<Type, ITaskQueue> taskQueues = new Dictionary<Type, ITaskQueue>();
    private readonly Dictionary<Type, Action> taskQueueUnsubscribers = new Dictionary<Type, Action>();
    private IApp app = null!;
    #endregion
}
