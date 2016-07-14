// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Factory
{
    /// <summary>
    /// Default factory which will execute the default constructor
    /// unless a different method has been registered.
    /// </summary>
    internal sealed class DefaultFactory : IFactory, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
        }

        public void Uninitialize()
        {
            this.factories.Clear();
        }
        #endregion

        #region IFactory implementation
        public void Register<T>(Func<T> factory)
        {
            this.RegisterInternal((Func<object>)(object)factory, typeof(T));
        }
        
        public object Get(Type type)
        {
            return this.factories.ContainsKey(type) ?
                this.factories[type]() :
                type.CreateObject();
        }
        #endregion

        #region Private methods
        private void RegisterInternal(Func<object> factory, Type type)
        {
            this.RegisterType(factory, type);
            this.RegisterBaseClasses(factory, type);
            this.RegisterInterfaces(factory, type);
        }

        private void RegisterBaseClasses(Func<object> factory, Type type)
        {
            var baseType = type.GetTypeInfo().BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                this.RegisterType(factory, baseType);
                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        private void RegisterInterfaces(Func<object> factory, Type type)
        {
            foreach (var i in type.GetTypeInfo().GetInterfaces())
            {
                this.RegisterType(factory, i);
            }
        }

        private void RegisterType(Func<object> factory, Type type)
        {
            this.factories[type] = factory;
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();
        #endregion
    }
}
