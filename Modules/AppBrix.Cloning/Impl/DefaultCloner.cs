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
        private object DeepCopy(object original, Dictionary<object, object> visited)
        {
            if (original == null)
                return null;

            if (original is Type)
                return original;

            var type = original.GetType();

            if (this.IsPrimitiveType(type))
                return original;

            if (!visited.ContainsKey(original))
            {
                this.CloneReferenceType(original, type, visited);
            }

            return visited[original];
        }

        private void CloneReferenceType(object original, Type type, Dictionary<object, object> visited)
        {
            var cloned = this.ShallowCopy(original);
            visited[original] = cloned;
            if (type.IsArray)
            {
                var clonedArray = (Array)cloned;
                this.ForEach((Array)original, (item, indices) => clonedArray.SetValue(this.DeepCopy(item, visited), indices));
            }

            var baseType = type;
            while (baseType != typeof(object) && baseType != null)
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

        private void ForEach(Array array, Action<object, int[]> action)
        {
            var length = array.Length;
            if (length == 0)
                return;

            var position = new int[array.Rank];
            var lowerBounds = new int[array.Rank];
            var upperBounds = new int[array.Rank];
            var lastIndex = array.Rank - 1;
            for (int i = 0; i <= lastIndex; i++)
            {
                position[i] = lowerBounds[i] = array.GetLowerBound(i);
                upperBounds[i] = array.GetUpperBound(i);
            }

            action(array.GetValue(position), position);
            for (int i = 1; i < length; i++)
            {
                position[lastIndex]++;
                for (var index = lastIndex; index > 0 && position[index] > upperBounds[index]; index--)
                {
                    position[index] = lowerBounds[index];
                    position[index - 1]++;
                }
                action(array.GetValue(position), position);
            }
        }
        #endregion

        #region Private fields and constants
        private static readonly MethodInfo ShallowCopyMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
    }
}
