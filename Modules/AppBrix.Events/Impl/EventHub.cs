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
            this.subscriptions[type] = handlers = new EventsWrapper<T>();

        ((IEventsWrapper<T>)handlers).Subscribe(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        var type = typeof(T);
        if (this.subscriptions.TryGetValue(type, out var h) && h is IEventsWrapper<T> handlers)
        {
            handlers.Unsubscribe(handler);
            if (handlers.IsEmpty)
                this.subscriptions.Remove(type);
        }
    }

    public void Raise(IEvent args)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        var type = args.GetType();
        if (!this.interfaces.TryGetValue(type, out var types))
            this.interfaces[type] = types = this.GetEventTypes(type);

        foreach (var eventType in types)
        {
            if (this.subscriptions.TryGetValue(eventType, out var handlers))
                handlers.Execute(args);
        }
    }
    #endregion

    #region Private methods
    private Type[] GetEventTypes(Type type)
    {
        var typeList = new List<Type>();

        for (var baseType = type; baseType != typeof(object); baseType = baseType.BaseType!)
        {
            if (baseType.IsAssignableTo(typeof(IEvent)))
                typeList.Add(baseType);
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsAssignableTo(typeof(IEvent)))
                typeList.Add(interfaceType);
        }

        return typeList.ToArray();
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<Type, Type[]> interfaces = new Dictionary<Type, Type[]>();
    private readonly Dictionary<Type, IEventsWrapper> subscriptions = new Dictionary<Type, IEventsWrapper>();
    #endregion
}
