// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl;

/// <summary>
/// An asynchronous task runner.
/// </summary>
internal sealed class TaskQueue<T> : ITaskQueue<T>
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
        this.runner = this.Run(this.cts.Token);
    }
    #endregion

    #region Properties
    public int Count => this.handlers.Count;
    #endregion

    #region Public and overriden methods
    public void Dispose()
    {
        if (!this.isDisposed)
        {
            this.isDisposed = true;
            this.cts?.Cancel();
            this.cts = null;
            this.channel.Writer.Complete();
            this.handlers.Clear();
        }
    }

    public void Subscribe(Action<T> handler) => this.handlers.Add(handler);

    public void Unsubscribe(Action<T> handler)
    {
        // Optimize for unsubscribing the last element since this is the most common scenario.
        for (var i = this.handlers.Count - 1; i >= 0; i--)
        {
            if (this.handlers[i].Equals(handler))
            {
                this.handlers.RemoveAt(i);
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
            for (var i = 0; i < this.handlers.Count; i++)
            {
                Action<T>? handler = null;
                try
                {
                    handler = this.handlers[i];
                    handler(args);
                }
                catch (Exception) { }
        
                if (handler is not null && i < this.handlers.Count)
                {
                    try
                    {
                        // Check if the handler has unsubscribed itself.
                        if (!object.ReferenceEquals(handler, this.handlers[i]))
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
    private readonly List<Action<T>> handlers = new List<Action<T>>();
    private readonly Task runner;
    private bool isDisposed;
    #endregion
}
