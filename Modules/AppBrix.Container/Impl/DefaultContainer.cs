// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Container.Impl
{
    /// <summary>
    /// Default container which will be used when no other container has been registered.
    /// To override, register your container immediately after the ContainerModule registers this.
    /// </summary>
    internal sealed class DefaultContainer : IContainer, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.Register(context.App, context.App.GetType());
            this.Register(context.App.ConfigService, context.App.ConfigService.GetType());
            this.Register(this);
        }

        public void Uninitialize()
        {
            this.registered.Clear();
            this.objects.Clear();
        }
        #endregion

        #region IContainer implementation
        public void Register(object obj, Type type)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type == typeof(object))
                throw new ArgumentException($"Cannot register object as type {typeof(object).FullName}.");
            if (type == typeof(string))
                throw new ArgumentException($"Cannot register object as type {typeof(string).FullName}.");
            if (type.IsValueType)
                throw new ArgumentException("Container does not support value types.");
            if (!type.IsInstanceOfType(obj))
                throw new ArgumentException($"Target object is of type {obj.GetType().FullName} which cannot be cast to target type {type.FullName}.");

            this.registered.Add(obj);
            this.RegisterInternal(obj, type);
        }
        
        public object Get(Type type)
        {
            return this.objects[type];
        }

        public IEnumerable<object> GetAll()
        {
            return this.registered;
        }
        #endregion

        #region Private methods
        private void RegisterInternal(object obj, Type type)
        {
            var baseType = type;
            while (baseType != typeof(object) && baseType != null)
            {
                this.objects[baseType] = obj;
                baseType = baseType.BaseType;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                this.objects[@interface] = obj;
            }
        }
        #endregion

        #region Private fields and constants
        private readonly Dictionary<Type, object> objects = new Dictionary<Type, object>();
        private readonly List<object> registered = new List<object>();
        #endregion
    }
}
