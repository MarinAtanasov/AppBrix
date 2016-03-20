// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Entries;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Logging.Loggers
{
    /// <summary>
    /// A logger which logs the entries a separate thread.
    /// </summary>
    internal sealed class AsyncLogger : Logger, IDisposable
    {
        #region Construciton
        /// <summary>
        /// Creates an asynchronous logger using the provider log writer.
        /// </summary>
        /// <param name="writer">The log writer that will log the log entries.</param>
        public AsyncLogger(ILogWriter writer) : base(writer)
        {
        }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Initializes a new thread which will be used for logging.
        /// Initializes the logger and subscribes to the log entry events.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void Initialize(IInitializeContext context)
        {
            if (this.task != null)
                throw new InvalidOperationException("Logger already initialized.");

            this.logQueue = new BlockingCollection<ILogEntry>();
            this.cancelTokenSource = new CancellationTokenSource();
            this.task = Task.Factory.StartNew(() =>
            {
                foreach (var entry in this.logQueue.GetConsumingEnumerable())
                {
                    // Throw only after flushing the entries in the queue.
                    if (entry == null)
                        this.cancelTokenSource.Token.ThrowIfCancellationRequested();
                    this.writer.WriteEntry(entry);
                }
            }, this.cancelTokenSource.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
            base.Initialize(context);
        }

        /// <summary>
        /// Disposes of the thread and unsubscribes from the log entry events.
        /// </summary>
        public override void Uninitialize()
        {
            this.Dispose();
        }

        /// <summary>
        /// Code analysis CA1001: Types that own disposable fields should be disposable.
        /// Do not use. Use Uninitialize instead.
        /// </summary>
        public void Dispose()
        {
            if (this.task != null)
            {
                this.cancelTokenSource.Cancel();
                // We need to add another element to trigger the enumerator
                // in order to cancel the task and dispose of the thread.
                this.logQueue.Add(null);
                try { this.task.Wait(); }
                catch (AggregateException) { }

                base.Uninitialize();
                this.cancelTokenSource.Dispose();
                this.cancelTokenSource = null;
                this.logQueue.Dispose();
                this.logQueue = null;
                this.task = null;
            }
        }

        /// <summary>
        /// Enqueues a log entry to the queue to be logged asynchronously.
        /// </summary>
        /// <param name="entry">The log entry which will be logged.</param>
        public override void LogEntry(ILogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            this.logQueue.Add(entry);
        }
        #endregion

        #region Private fields and constants
        private BlockingCollection<ILogEntry> logQueue;
        private CancellationTokenSource cancelTokenSource;
        private Task task;
        #endregion
    }
}
