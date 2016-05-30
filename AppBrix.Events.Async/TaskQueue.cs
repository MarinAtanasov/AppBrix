// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Events.Async
{
    /// <summary>
    /// An asynchronous task runner.
    /// </summary>
    internal sealed class TaskQueue<T> : ITaskQueue, IDisposable
    {
        #region Construciton
        /// <summary>
        /// Creates an asynchronous runner queue.
        /// </summary>
        public TaskQueue()
        {
            this.tasks = new BlockingCollection<T>();
            this.cancelTokenSource = new CancellationTokenSource();
            this.runner = Task.Factory.StartNew(() =>
            {
                foreach (var args in this.tasks.GetConsumingEnumerable())
                {
                    // Throw only after flushing the queue.
                    if (args == null)
                        this.cancelTokenSource.Token.ThrowIfCancellationRequested();

                    for (var i = 0; i < this.handlers.Count; i++)
                    {
                        var handler = this.handlers[i];
                        handler(args);

                        // Check if the handler has unsubscribed itself.
                        if (i < handlers.Count && !object.ReferenceEquals(handler, handlers[i]))
                            i--;
                    }
                }
            }, this.cancelTokenSource.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets how many handlers are subscribed in the queue.
        /// </summary>
        public int Count => this.handlers.Count;
        #endregion

        #region Public and overriden methods
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                this.cancelTokenSource.Cancel();
                // We need to add another element to trigger the enumerator
                // in order to cancel the runner and dispose of the thread.
                object exitTask = null;
                this.tasks.Add((T)exitTask);
                try { this.runner.Wait(); }
                catch (AggregateException) { }

                this.cancelTokenSource.Dispose();
                this.tasks.Dispose();
                this.handlers.Clear();
            }
        }

        void ITaskQueue.Subscribe(Action<IEvent> handler)
        {
            object obj = handler;
            this.Subscribe((Action<T>)obj);
        }

        public void Subscribe(Action<T> handler)
        {
            this.handlers.Add(handler);
        }

        void ITaskQueue.Unsubscribe(Action<IEvent> handler)
        {
            object obj = handler;
            this.Unsubscribe((Action<T>)obj);
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

        void ITaskQueue.Enqueue(IEvent args)
        {
            this.Enqueue((T)args);
        }
        
        public void Enqueue(T task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            this.tasks.Add(task);
        }
        #endregion

        #region Private fields and constants
        private readonly CancellationTokenSource cancelTokenSource;
        private readonly IList<Action<T>> handlers = new List<Action<T>>();
        private readonly Task runner;
        private readonly BlockingCollection<T> tasks;
        private bool isDisposed;
        #endregion
    }
}
