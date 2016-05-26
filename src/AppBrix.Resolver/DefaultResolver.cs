// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Resolver
{
    /// <summary>
    /// Default resolver which will be used when no other resolver has been registered.
    /// To override, register your resolver immediately after the ResolverModule registers this.
    /// </summary>
    internal sealed class DefaultResolver : IResolver, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.Register(context.App, context.App.GetType());
            this.Register(context.App.ConfigManager, context.App.ConfigManager.GetType());
            this.Register(this);
        }

        public void Uninitialize()
        {
            this.registered.Clear();
            this.objects.Clear();
        }
        #endregion

        #region IResolver implementation
        public void Register(object obj, Type type)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.GetTypeInfo().IsInstanceOfType(obj))
                throw new ArgumentException(string.Format(
                    "Target object is of type {0} which cannot be cast to target type {1}.",
                    obj.GetType().FullName, type.FullName));
            if (type == typeof(object))
                throw new ArgumentException($"Cannot register object as type {typeof(object).FullName}.");

            this.registered.Add(obj);
            this.RegisterInternal(obj, type);
        }

        public T Get<T>() where T : class
        {
            try
            {
                return (T)this.objects[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(
                    $"Object of type '{typeof(T).GetAssemblyQualifiedName()}' has not been registered.");
            }
        }

        public IEnumerable<object> GetAll()
        {
            return this.registered;
        }
        #endregion

        #region Private methods
        private void RegisterInternal(object obj, Type type)
        {
            this.RegisterType(obj, type);
            this.RegisterBaseClasses(obj, type);
            this.RegisterInterfaces(obj, type);
        }

        private void RegisterBaseClasses(object obj, Type type)
        {
            var baseType = type.GetTypeInfo().BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                this.RegisterType(obj, baseType);
                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        private void RegisterInterfaces(object obj, Type type)
        {
            foreach (var i in type.GetTypeInfo().GetInterfaces())
            {
                this.RegisterType(obj, i);
            }
        }

        private void RegisterType(object obj, Type type)
        {
            this.objects[type] = (obj);
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<Type, object> objects = new Dictionary<Type, object>();
        private readonly HashSet<object> registered = new HashSet<object>();
        #endregion
    }
}
