// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Impl
{
    internal abstract class EventWrapper
    {
        public EventWrapper(object handler)
        {
            this.Handler = handler;
        }

        public object Handler { get; }

        public abstract void Execute(IEvent args);
    }

    internal sealed class EventWrapper<T> : EventWrapper where T : IEvent
    {
        public EventWrapper(Action<T> handler) : base(handler)
        {
            this.handler = handler;
        }

        public sealed override void Execute(IEvent args)
        {
            this.handler((T)args);
        }

        private readonly Action<T> handler;
    }
}
