// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Cloning
{
    /// <summary>
    /// Default cloner used for shallow or deep copying.
    /// </summary>
    internal sealed class DefaultCloner : ICloner
    {
        #region ICloner implementation
        public T DeepCopy<T>(T obj)
        {
            return this.DeepCopy(obj, new Dictionary<object, object>());
        }

        public T ShallowCopy<T>(T obj)
        {
            if (this.IsValueType(typeof(T)))
                return obj;

            return (T)DefaultCloner.ShallowCopyMethod.Invoke(obj, null);
        }
        #endregion

        #region Private methods
        private T DeepCopy<T>(T original, IDictionary<object, object> visited)
        {
            if (original == null)
                // ReSharper disable once ExpressionIsAlwaysNull
                return original;

            var type = original.GetType();

            if (this.IsPrimitiveType(type))
                return original;

            if (typeof(Delegate).GetTypeInfo().IsAssignableFrom(type))
                return (T)(object)null;

            if (!visited.ContainsKey(original))
            {
                this.CloneReferenceType(original, type, visited);
            }

            return (T)visited[original];
        }

        private void CloneReferenceType(object original, Type type, IDictionary<object, object> visited)
        {
            var cloned = this.ShallowCopy(original);
            visited[original] = cloned;
            if (type.IsArray)
            {
                var clonedArray = (Array)cloned;
                this.ForEach(((Array)original), (array, indices) => clonedArray.SetValue(this.DeepCopy(array.GetValue(indices), visited), indices));
            }
            while (type != typeof(object))
            {
                foreach (var field in type.GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    field.SetValue(cloned, this.DeepCopy(field.GetValue(original), visited));
                }
                type = type.GetTypeInfo().BaseType;
            }
        }

        private bool IsPrimitiveType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return (typeInfo.IsValueType && typeInfo.IsPrimitive) || typeInfo.IsEnum || type == typeof(string);
        }

        private bool IsValueType(Type type)
        {
            return type == typeof(string) || type.GetTypeInfo().IsValueType;
        }

        /// <summary>
        /// Traverses a multidimentional array and executes an action for every item.
        /// </summary>
        /// <param name="array">The array to be traversed.</param>
        /// <param name="action">The action to be executed on every element.</param>
        private void ForEach(Array array, Action<Array, int[]> action)
        {
            if (array.Length == 0)
                return;

            var walker = new ArrayTraverse(array);
            do
            {
                action(array, walker.Position);
            }
            while (walker.Step());
        }
        #endregion

        #region Private fields and constants
        private static readonly MethodInfo ShallowCopyMethod = typeof(object).GetTypeInfo().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
    }
}
