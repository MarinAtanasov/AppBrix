// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Services;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;

namespace AppBrix.Events.Impl;

internal sealed class EventHub : IEventHub, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
    }

    public void Uninitialize()
    {
        this.subscriptions.Clear();
    }
    #endregion

    #region IEventHub implementation
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        var type = typeof(T);
        if (!this.subscriptions.TryGetValue(type, out var handlers))
            this.subscriptions[type] = handlers = new List<EventWrapper>();

        handlers.Add(new EventWrapper<T>(handler));
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        if (this.subscriptions.TryGetValue(typeof(T), out var handlers))
        {
            // Optimize for unsubscribing the last element since this is the most common scenario.
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].Handler.Equals(handler))
                {
                    handlers.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void Raise(IEvent args)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        var type = args.GetType();
        for (var baseType = type; baseType != typeof(object); baseType = baseType.BaseType!)
        {
            this.RaiseEvent(args, baseType);
        }
        
        var interfaces = type.GetInterfaces();
        for (var i = 0; i < interfaces.Length; i++)
        {
            this.RaiseEvent(args, interfaces[i]);
        }
    }
    #endregion

    #region Private methods
    private void RaiseEvent(IEvent args, Type eventType)
    {
        if (this.subscriptions.TryGetValue(eventType, out var handlers))
        {
            for (var i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];
                handler.Execute(args);

                // Check if the handler has unsubscribed itself.
                if (i < handlers.Count && !object.ReferenceEquals(handler, handlers[i]))
                    i--;
            }
        }
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<Type, List<EventWrapper>> subscriptions = new Dictionary<Type, List<EventWrapper>>();
    #endregion
}
