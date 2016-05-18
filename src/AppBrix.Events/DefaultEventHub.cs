// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Events
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

            this.SubscribeInternal(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            this.UnsubscribeInternal(handler);
        }

        public void Raise<T>(T args) where T : IEvent
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            this.RaiseEvent(args, typeof(T));
            this.RaiseBaseClassesEvents(args);
            this.RaiseInterfacesEvents(args);
        }
        #endregion

        #region Private methods
        private void SubscribeInternal<T>(Action<T> handler) where T : IEvent
        {
            if (!this.subscriptions.ContainsKey(typeof(T)))
                this.subscriptions[typeof(T)] = new List<object>();

            this.subscriptions[typeof(T)].Add(handler);
        }

        private void UnsubscribeInternal<T>(Action<T> handler) where T : IEvent
        {
            if (this.subscriptions.ContainsKey(typeof(T)))
            {
                // Optimize for unsubscribing the last element since this is the most common scenario.
                var handlers = this.subscriptions[typeof(T)];
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    if (handlers[i].Equals(handler))
                    {
                        handlers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void RaiseBaseClassesEvents<T>(T args) where T : IEvent
        {
            var baseType = typeof(T).GetTypeInfo().BaseType;
            while (baseType != null && typeof(IEvent).GetTypeInfo().IsAssignableFrom(baseType))
            {
                this.RaiseEvent(args, baseType);
                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        private void RaiseInterfacesEvents<T>(T args) where T : IEvent
        {
            foreach (var iface in typeof(T).GetTypeInfo().GetInterfaces().Where(i => typeof(IEvent).GetTypeInfo().IsAssignableFrom(i)))
            {
                this.RaiseEvent(args, iface);
            }
        }

        private void RaiseEvent<T>(T args, Type eventType) where T : IEvent
        {
            if (this.subscriptions.ContainsKey(eventType))
            {
                var handlers = this.subscriptions[eventType];
                for (var i = 0; i < handlers.Count; i++)
                {
                    var handler = (Action<T>)handlers[i];
                    handler(args);

                    // Check if the handler has unsubscribed itself.
                    if (i < handlers.Count && !Object.ReferenceEquals(handler, handlers[i]))
                        i--;
                }
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<Type, IList<object>> subscriptions = new Dictionary<Type, IList<object>>();
        #endregion
    }
}
