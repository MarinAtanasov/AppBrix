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
    /// Default cloner used for shalow or deep copying.
    /// </summary>
    internal sealed class DefaultCloner : ICloner
    {
        #region ICloner implementation
        public T DeepCopy<T>(T obj)
        {
            return this.DeepCopy(obj, new Dictionary<object, object>());
        }

        public T ShalowCopy<T>(T obj)
        {
            return (T)DefaultCloner.ShalowCopyMethod.Invoke(obj, null);
        }
        #endregion

        #region Private methods
        private T DeepCopy<T>(T original, IDictionary<object, object> visited)
        {
            if (original == null)
                return original;

            var type = original.GetType();

            if (this.IsPrimitiveType(type))
                return original;

            if (typeof(Delegate).IsAssignableFrom(type))
                return (T)(object)null;

            if (!visited.ContainsKey(original))
            {
                this.CloneReferenceType(original, type, visited);
            }

            return (T)visited[original];
        }

        private object CloneReferenceType(object original, Type type, IDictionary<object, object> visited)
        {
            var cloned = this.ShalowCopy(original);
            visited[original] = cloned;
            if (type.IsArray)
            {
                var clonedArray = (Array)cloned;
                ((Array)original).ForEach((array, indices) => clonedArray.SetValue(this.DeepCopy(array.GetValue(indices), visited), indices));
            }
            while (type != typeof(object))
            {
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    field.SetValue(cloned, this.DeepCopy(field.GetValue(original), visited));
                }
                type = type.BaseType;
            }
            return cloned;
        }

        private bool IsPrimitiveType(Type type)
        {
            return type == typeof(string) || type.IsEnum || (type.IsValueType && type.IsPrimitive);
        }
        #endregion

        #region Private fields and constants
        private static readonly MethodInfo ShalowCopyMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
    }
}
