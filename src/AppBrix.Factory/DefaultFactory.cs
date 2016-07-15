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
            var hierarchy = type.GetClassHierarchy()
                .Where(c => c != typeof(object))
                .Concat(type.GetTypeInfo().GetInterfaces());

            foreach (var item in hierarchy)
            {
                this.factories[item] = factory;
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();
        #endregion
    }
}
