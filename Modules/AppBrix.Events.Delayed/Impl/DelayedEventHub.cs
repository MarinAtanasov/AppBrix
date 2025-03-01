// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Delayed.Configuration;
using AppBrix.Events.Delayed.Contracts;
using AppBrix.Events.Delayed.Services;
using AppBrix.Events.Services;
using AppBrix.Lifecycle;
using System;
using System.Threading;
using System.Threading.Channels;

namespace AppBrix.Events.Delayed.Impl;

internal sealed class DelayedEventHub : IDelayedEventHub, IApplicationLifecycle
{
    #region Properties
    public IEventHub EventHub { get; private set; } = null!;
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
        if (this.channel is not null)
        {
            this.channel.Writer.Complete();
            this.Flush();
        }

        this.EventHub = null!;
        this.config = null!;
        this.channel = null!;
        this.app = null!;
    }
    #endregion

    #region IEventHub implementation

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        this.EventHub.Subscribe(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        this.EventHub.Unsubscribe(handler);
    }

    public void Raise(IEvent args)
    {
        switch (args)
        {
            case IDelayedEvent:
                this.RaiseDelayed(args);
                return;
            case IImmediateEvent:
                this.RaiseImmediate(args);
                return;
        }

        switch (this.config.DefaultBehavior)
        {
            case EventBehavior.Immediate:
                this.RaiseImmediate(args);
                break;
            case EventBehavior.Delayed:
                this.RaiseDelayed(args);
                break;
            default:
                throw new InvalidOperationException($"{nameof(this.config.DefaultBehavior)}: {this.config.DefaultBehavior}");
        }
    }
    #endregion

    #region IDelayedEventHub implementation
    public void Flush()
    {
        lock (this.flushLock)
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

    public void RaiseImmediate(IEvent args)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        this.EventHub.Raise(args);
    }
    #endregion

    #region Private fields and constants
    private readonly Lock flushLock = new Lock();
    private IApp app = null!;
    private Channel<IEvent> channel = null!;
    private DelayedEventsConfig config = null!;
    #endregion
}
