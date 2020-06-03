// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Delayed.Configuration;
using AppBrix.Lifecycle;
using System;
using System.Threading.Channels;

namespace AppBrix.Events.Delayed.Impl
{
    internal sealed class DefaultDelayedEventHub : IDelayedEventHub, IApplicationLifecycle
    {
        #region Properties
        #nullable disable
        public IEventHub EventHub { get; private set; }
        #nullable restore
        #endregion

        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
            this.channel = Channel.CreateUnbounded<IEvent>(new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = true,
                SingleReader = true,
                SingleWriter = false
            });
            this.config = this.app.ConfigService.GetDelayedEventsConfig();
            this.EventHub = this.app.GetEventHub();
        }

        public void Uninitialize()
        {
            if (this.channel != null)
            {
                this.channel.Writer.Complete();
                this.Flush();
            }

            this.EventHub = null;
            this.config = null;
            this.channel = null;
            this.app = null;
        }
        #endregion

        #region IEventHub implementation
        public void Subscribe<T>(Action<T> handler) where T : IEvent => this.EventHub.Subscribe(handler);

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent => this.EventHub.Unsubscribe(handler);

        public void Raise(IEvent args)
        {
            switch (this.config.DefaultBehavior)
            {
                case EventBehavior.Immediate:
                    this.RaiseImmediate(args);
                    break;
                case EventBehavior.Delayed:
                    this.RaiseDelayed(args);
                    break;
                default:
                    throw new InvalidOperationException($@"{nameof(this.config.DefaultBehavior)}: {this.config.DefaultBehavior}");
            }
        }
        #endregion

        #region IDelayedEventHub implementation
        public void Flush()
        {
            lock (this.channel)
            {
                var reader = this.channel.Reader;
                while (reader.TryRead(out var args))
                {
                    this.RaiseImmediate(args);
                }
            }
        }

        public void RaiseDelayed(IEvent args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            this.channel.Writer.TryWrite(args);
        }

        public void RaiseImmediate(IEvent args) => this.EventHub.Raise(args);
        #endregion

        #region Private methods
        #endregion

        #region Private fields and constants
        #nullable disable
        private IApp app;
        private Channel<IEvent> channel;
        private DelayedEventsConfig config;
        #nullable restore
        #endregion
    }
}
