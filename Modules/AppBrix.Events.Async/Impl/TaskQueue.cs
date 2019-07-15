// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Impl
{
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
            this.runner = this.Run();
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

                this.channel.Writer.Complete();
                try { this.runner.Wait(); }
                catch (AggregateException) { }

                this.handlers.Clear();
            }
        }
        
        public void Subscribe(Action<T> handler)
        {
            this.handlers.Add(handler);
        }
        
        public void Unsubscribe(Action<T> handler)
        {
            // Optimize for unsubscribing the last element since this is the most common scenario.
            for (int i = this.handlers.Count - 1; i >= 0; i--)
            {
                if (this.handlers[i].Equals(handler))
                {
                    this.handlers.RemoveAt(i);
                    break;
                }
            }
        }
        
        public void Enqueue(T task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            this.channel.Writer.TryWrite(task);
        }
        #endregion

        #region Private methods
        private async Task Run()
        {
            var reader = this.channel.Reader;
            while (await reader.WaitToReadAsync().ConfigureAwait(false))
            while (reader.TryRead(out var args))
            {
                for (var i = 0; i < this.handlers.Count; i++)
                {
                    try
                    {
                        var handler = this.handlers[i];
                        handler(args);

                        // Check if the handler has unsubscribed itself.
                        if (i < handlers.Count && !object.ReferenceEquals(handler, handlers[i]))
                            i--;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        #endregion

        #region Private fields and constants
        private readonly Channel<T> channel;
        private readonly List<Action<T>> handlers = new List<Action<T>>();
        private readonly Task runner;
        private bool isDisposed;
        #endregion
    }
}
