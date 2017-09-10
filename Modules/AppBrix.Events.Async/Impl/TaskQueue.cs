// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            this.tasks = new BlockingCollection<T>();
            this.cancelTokenSource = new CancellationTokenSource();
            this.runner = Task.Factory.StartNew(this.Run, this.cancelTokenSource.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
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

            this.tasks.Add(task);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Iterates over the scheduled tasks and executes them in order. Should be called only from the constructor.
        /// This method executes continuously inside a separate thread until it is cancelled during <see cref="Dispose"/>.
        /// </summary>
        private void Run()
        {
            foreach (var args in this.tasks.GetConsumingEnumerable())
            {
                // Throw only after flushing the queue.
                if (args == null)
                    this.cancelTokenSource.Token.ThrowIfCancellationRequested();

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
        private readonly CancellationTokenSource cancelTokenSource;
        private readonly List<Action<T>> handlers = new List<Action<T>>();
        private readonly Task runner;
        private readonly BlockingCollection<T> tasks;
        private bool isDisposed;
        #endregion
    }
}
