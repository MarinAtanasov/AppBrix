// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Events.Impl
{
    internal sealed class DefaultEventHub : IEventHub, IApplicationLifecycle
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
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            this.SubscribeInternal(new EventWrapper(handler, args => handler((T)args)), typeof(T));
        }
        
        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            this.UnsubscribeInternal(handler, typeof(T));
        }

        public void Raise(IEvent args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            this.RaiseInternal(args, args.GetType());
        }
        #endregion

        #region Private methods
        private void SubscribeInternal(EventWrapper handler, Type type)
        {
            List<EventWrapper> handlers;
            if (!this.subscriptions.TryGetValue(type, out handlers))
            {
                handlers = new List<EventWrapper>();
                this.subscriptions[type] = handlers;
            }
            handlers.Add(handler);
        }

        private void UnsubscribeInternal(object handler, Type type)
        {
            if (this.subscriptions.TryGetValue(type, out var handlers))
            {
                // Optimize for unsubscribing the last element since this is the most common scenario.
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    if (handlers[i].Handler.Equals(handler))
                    {
                        handlers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void RaiseInternal(object args, Type type)
        {
            var baseType = type;
            while (baseType != typeof(object) && baseType != null)
            {
                this.RaiseEvent(args, baseType);
                baseType = baseType.BaseType;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                this.RaiseEvent(args, @interface);
            }
        }

        private void RaiseEvent(object args, Type eventType)
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
}
