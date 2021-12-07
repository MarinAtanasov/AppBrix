// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;

namespace AppBrix.Events.Impl;

internal abstract class EventWrapper
{
    public abstract object Handler { get; }

    public abstract void Execute(IEvent args);
}

internal sealed class EventWrapper<T> : EventWrapper where T : IEvent
{
    public EventWrapper(Action<T> handler)
    {
        this.handler = handler;
    }

    public override object Handler => this.handler;

    public override void Execute(IEvent args) => this.handler((T)args);

    private readonly Action<T> handler;
}
