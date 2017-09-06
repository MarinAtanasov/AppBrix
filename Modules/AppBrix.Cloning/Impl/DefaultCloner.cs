// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Cloning.Impl
{
    /// <summary>
    /// Default cloner used for shallow or deep copying.
    /// </summary>
    internal sealed class DefaultCloner : ICloner
    {
        #region ICloner implementation
        public object DeepCopy(object obj)
        {
            return this.DeepCopy(obj, new Dictionary<object, object>());
        }

        public object ShallowCopy(object obj)
        {
            if (obj == null || this.IsValueType(obj.GetType()))
                return obj;

            return DefaultCloner.ShallowCopyMethod.Invoke(obj, null);
        }
        #endregion

        #region Private methods
        private object DeepCopy(object original, IDictionary<object, object> visited)
        {
            if (original == null)
                return null;

            var type = original.GetType();

            if (this.IsPrimitiveType(type))
                return original;

            if (this.IsDelegate(type))
                return null;

            if (!visited.ContainsKey(original))
            {
                this.CloneReferenceType(original, type, visited);
            }

            return visited[original];
        }

        private void CloneReferenceType(object original, Type type, IDictionary<object, object> visited)
        {
            var cloned = this.ShallowCopy(original);
            visited[original] = cloned;
            if (type.IsArray)
            {
                var clonedArray = (Array)cloned;
                this.ForEach((Array)original, (array, indices) => clonedArray.SetValue(this.DeepCopy(array.GetValue(indices), visited), indices));
            }

            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                var fields = baseType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in fields)
                {
                    field.SetValue(cloned, this.DeepCopy(field.GetValue(original), visited));
                }
                baseType = baseType.BaseType;
            }
        }

        private bool IsPrimitiveType(Type type)
        {
            return (type.IsValueType && type.IsPrimitive) ||
                   type.IsEnum || type == typeof(string) || type == typeof(DateTime);
        }

        private bool IsValueType(Type type)
        {
            return type == typeof(string) || type.IsValueType;
        }

        private bool IsDelegate(Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
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
        private static readonly MethodInfo ShallowCopyMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
    }
}
