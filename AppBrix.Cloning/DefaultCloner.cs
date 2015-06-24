// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Cloning
{
    /// <summary>
    /// Default factory which will execute the default constructor
    /// unless a different method has been registered.
    /// </summary>
    internal sealed class DefaultCloner : ICloner, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.app = null;
        }
        #endregion

        #region ICloner implementation
        public T Clone<T>(T obj)
        {
            return new ClonerInternal().Clone<T>(obj);
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion

        #region Private classes
        private class ClonerInternal
        {
            #region Public methods
            public T Clone<T>(T original)
            {
                if (original == null)
                    return original;

                var type = original.GetType();

                if (this.IsPrimitiveType(type) || this.IsSpecialType(type))
                    return original;

                if (typeof(Delegate).IsAssignableFrom(type))
                    return (T)(object)null;

                if (!this.visited.ContainsKey(original))
                {
                    this.CloneReferenceType(original, type);
                }

                return (T)this.visited[original];
            }
            #endregion

            #region Private methods
            private object CloneReferenceType(object original, Type type)
            {
                var cloned = ClonerInternal.CloneMethod.Invoke(original, null);
                this.visited[original] = cloned;
                if (type.IsArray)
                {
                    var clonedArray = (Array)cloned;
                    ((Array)original).ForEach((array, indices) => clonedArray.SetValue(this.Clone(array.GetValue(indices)), indices));
                }
                while (type != typeof(object))
                {
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public))
                    {
                        field.SetValue(cloned, this.Clone(field.GetValue(original)));
                    }
                    type = type.BaseType;
                }
                return cloned;
            }

            private bool IsPrimitiveType(Type type)
            {
                return type == typeof(string) || type.IsEnum || (type.IsValueType && type.IsPrimitive);
            }

            private bool IsSpecialType(Type type)
            {
                return type.FullName.StartsWith("System.Reflection") || type.FullName.StartsWith("System.Runtime");
            }
            #endregion

            #region Private fields and constants
            private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            private IDictionary<object, object> visited = new Dictionary<object, object>(new ReferenceEqualityComparer());
            #endregion
        }
        #endregion
    }
}
