// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Cloning.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Cloning.Tests
{
    [TestClass]
    public class CloningTests
    {
        #region Tests
        [TestMethod]
        public void TestDeepCopyInteger()
        {
            var cloner = this.GetCloner();
            var original = 5;
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [TestMethod]
        public void TestShalowCopyInteger()
        {
            var cloner = this.GetCloner();
            var original = 5;
            var clone = cloner.DeepCopy(original);
            this.AssertIsShalowCopy(original, clone);
        }

        [TestMethod]
        public void TestDeepCopyString()
        {
            var cloner = this.GetCloner();
            var original = "Test";
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [TestMethod]
        public void TestShalowCopyString()
        {
            var cloner = this.GetCloner();
            var original = "Test";
            var clone = cloner.DeepCopy(original);
            this.AssertIsShalowCopy(original, clone);
        }

        [TestMethod]
        public void TestDeepCopyNumericPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [TestMethod]
        public void TestShalowCopyNumericPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
            var clone = cloner.ShalowCopy(original);
            this.AssertIsShalowCopy(original, clone);
        }

        [TestMethod]
        public void TestDeepCopyPrimitivePropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now);
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [TestMethod]
        public void TestShalowCopyPrimitivePropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now);
            var clone = cloner.ShalowCopy(original);
            this.AssertIsShalowCopy(original, clone);
        }

        [TestMethod]
        public void TestDeepCopyComplexPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new ComplexPropertiesMock(10);
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [TestMethod]
        public void TestShalowCopyComplexPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new ComplexPropertiesMock(10);
            var clone = cloner.ShalowCopy(original);
            this.AssertIsShalowCopy(original, clone);
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceDeepCopy()
        {
            var cloner = this.GetCloner();
            for (int i = 0; i < 25; i++)
            {
                var original = new ComplexPropertiesMock(20);
                var clone = cloner.DeepCopy(original);
            }
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceShalowCopy()
        {
            var cloner = this.GetCloner();
            for (int i = 0; i < 1000; i++)
            {
                var original = new ComplexPropertiesMock(20);
                var clone = cloner.ShalowCopy(original);
            }
        }
        #endregion

        #region Private methods
        private ICloner GetCloner()
        {
            return new DefaultCloner();
        }

        private void AssertIsDeepCopy(object original, object copy)
        {
            if (original == null)
            {
                Assert.IsNull(copy, "Original is null but the copy is not.");
                return;
            }

            Assert.IsNotNull(copy, "Original is not null but the copy is.");

            var type = original.GetType();
            Assert.AreEqual(type, copy.GetType(), "The shallow clone is not of the same type.");

            if ((type.IsValueType && type.IsPrimitive) || type == typeof(string) || type.IsEnum)
            {
                Assert.AreEqual(original, copy, "Original and copied values are different.");
                if (type.IsPrimitive || type == typeof(string))
                    return;
            }
            else if (!type.IsValueType)
            {
                Assert.AreNotSame(original, copy, "Original and copied values are the same object.");
            }

            foreach (var field in this.GetFields(type))
            {
                this.AssertIsDeepCopy(field.GetValue(original), field.GetValue(copy));
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var originalEnumerator = ((IEnumerable)original).GetEnumerator();
                var copiedEnumerator = ((IEnumerable)copy).GetEnumerator();

                bool moveNext = originalEnumerator.MoveNext();
                Assert.AreEqual(moveNext, copiedEnumerator.MoveNext(),
                    string.Format("Original enumeration elements are {0}. Copied enumeration elements are {1}.",
                    ((IEnumerable)original).Cast<object>().Count(),
                    ((IEnumerable)copy).Cast<object>().Count()));
                while (moveNext)
                {
                    this.AssertIsDeepCopy(originalEnumerator.Current, copiedEnumerator.Current);
                    moveNext = originalEnumerator.MoveNext();
                    Assert.AreEqual(moveNext, copiedEnumerator.MoveNext(),
                        string.Format("Original enumeration elements are {0}. Copied enumeration elements are {1}.",
                        ((IEnumerable)original).Cast<object>().Count(),
                        ((IEnumerable)copy).Cast<object>().Count()));
                }
            }
        }

        private void AssertIsShalowCopy(object original, object copy, bool isInitialObject = true)
        {
            if (original == null)
            {
                Assert.IsNull(copy, "Original is null but the copy is not.");
                return;
            }

            Assert.IsNotNull(copy, "Original is not null but the copy is.");

            var type = original.GetType();
            Assert.AreEqual(type, copy.GetType(), "The shallow clone is not of the same type.");

            if (type.IsValueType || type == typeof(string))
            {
                Assert.AreEqual(original, copy, "Original and copied values are different.");
                if (type.IsPrimitive || type == typeof(string))
                    return;
            }
            else if (!isInitialObject)
            {
                Assert.AreSame(original, copy, "Original and copied values are different objects.");
            }

            foreach (var field in this.GetFields(type))
            {
                var originalField = field.GetValue(original);
                var copiedField = field.GetValue(copy);
                this.AssertIsShalowCopy(field.GetValue(original), field.GetValue(copy), false);
            }
        }

        private IEnumerable<FieldInfo> GetFields(Type type)
        {
            while (type != null)
	        {
                foreach (var field in type.GetFields(BindingFlags.Instance |
                    BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    yield return field;
                }
                type = type.BaseType;
	        }
        }
        #endregion
    }
}
