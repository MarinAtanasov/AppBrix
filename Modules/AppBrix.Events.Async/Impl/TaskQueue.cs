// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl;

/// <summary>
/// An asynchronous task runner.
/// </summary>
internal sealed class TaskQueue<T> : ITaskQueue<T> where T : IEvent
{
    #region Construciton
    /// <summary>
    /// Creates an asynchronous runner queue.
    /// </summary>
    public TaskQueue()
    {
        this.channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = true,
            SingleReader = true,
            SingleWriter = false
        });
        this.cts = new CancellationTokenSource();
        _ = this.Run(this.cts.Token);
    }
    #endregion

    #region Properties
    public int Count => this.items.Count;
    #endregion

    #region Public and overriden methods
    public void Dispose()
    {
        if (!this.isDisposed)
        {
            this.isDisposed = true;
            this.channel.Writer.Complete();
            this.cts?.Cancel();
            this.cts = null;
            this.items.Clear();
        }
    }

    public void Subscribe(Action<T> handler) => this.items.Add(new SyncTaskQueueItem<T>(handler));

    public void Subscribe(Func<T, Task> handler) => this.items.Add(new AsyncTaskQueueItem<T>(handler));

    public void Unsubscribe(Action<T> handler)
    {
        // Optimize for unsubscribing the last element since this is the most common scenario.
        for (var i = this.items.Count - 1; i >= 0; i--)
        {
            if (this.items[i] is SyncTaskQueueItem<T> item && item.Handler.Equals(handler))
            {
                this.items.RemoveAt(i);
                break;
            }
        }
    }

    public void Unsubscribe(Func<T, Task> handler)
    {
        // Optimize for unsubscribing the last element since this is the most common scenario.
        for (var i = this.items.Count - 1; i >= 0; i--)
        {
            if (this.items[i] is AsyncTaskQueueItem<T> item && item.Handler.Equals(handler))
            {
                this.items.RemoveAt(i);
                break;
            }
        }
    }

    public void Enqueue(T task) => this.channel.Writer.TryWrite(task);
    #endregion

    #region Private methods
    private async Task Run(CancellationToken token)
    {
        var reader = this.channel.Reader;
        while (await reader.WaitToReadAsync(token).ConfigureAwait(false))
        while (reader.TryRead(out var args))
        {
            for (var i = 0; i < this.items.Count && !token.IsCancellationRequested; i++)
            {
                ITaskQueueItem<T>? item = null;
                try
                {
                    item = this.items[i];
                    await item.Execute(args).ConfigureAwait(false);
                }
                catch (Exception) { }

                if (item is not null && i < this.items.Count)
                {
                    try
                    {
                        // Check if the handler has unsubscribed itself.
                        if (!object.ReferenceEquals(item, this.items[i]))
                            i--;
                    }
                    catch (Exception) { }
                }
            }
        }
    }
    #endregion

    #region Private fields and constants
    private CancellationTokenSource? cts;
    private readonly Channel<T> channel;
    private readonly List<ITaskQueueItem<T>> items = new List<ITaskQueueItem<T>>();
    private bool isDisposed;
    #endregion
}
