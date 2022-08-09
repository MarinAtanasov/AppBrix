// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;

namespace AppBrix.Events.Impl;

internal interface IEventsWrapper
{
    void Execute(IEvent args);
}

internal interface IEventsWrapper<out T> : IEventsWrapper where T : IEvent
{
    bool IsEmpty { get; }

    void Subscribe(Action<T> handler);
    
    void Unsubscribe(Action<T> handler);
}

internal sealed class EventsWrapper<T> : IEventsWrapper<T> where T : IEvent
{
    public bool IsEmpty => this.Handlers is null;

    private event Action<T>? Handlers;

    public void Subscribe(Action<T> handler) => this.Handlers += handler;

    public void Unsubscribe(Action<T> handler) => this.Handlers -= handler;

    public void Execute(IEvent args) => this.Handlers?.Invoke((T)args);
}
