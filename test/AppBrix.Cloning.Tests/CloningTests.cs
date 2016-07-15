// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Cloning.Tests.Mocks;
using AppBrix.Container;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AppBrix.Cloning.Tests
{
    public class CloningTests
    {
        #region Setup and cleanup
        public CloningTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(CloningModule),
                typeof(ContainerModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestDeepCopyInteger()
        {
            var cloner = this.GetCloner();
            var original = 5;
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [Fact]
        public void TestShallowCopyInteger()
        {
            var cloner = this.GetCloner();
            var original = 5;
            var clone = cloner.ShallowCopy(original);
            this.AssertIsShallowCopy(original, clone);
        }

        [Fact]
        public void TestDeepCopyString()
        {
            var cloner = this.GetCloner();
            var original = "Test";
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [Fact]
        public void TestShallowCopyString()
        {
            var cloner = this.GetCloner();
            var original = "Test";
            var clone = cloner.ShallowCopy(original);
            this.AssertIsShallowCopy(original, clone);
        }

        [Fact]
        public void TestDeepCopyNumericPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [Fact]
        public void TestShallowCopyNumericPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
            var clone = cloner.ShallowCopy(original);
            this.AssertIsShallowCopy(original, clone);
        }

        [Fact]
        public void TestDeepCopyPrimitivePropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now);
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [Fact]
        public void TestShallowCopyPrimitivePropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now);
            var clone = cloner.ShallowCopy(original);
            this.AssertIsShallowCopy(original, clone);
        }

        [Fact]
        public void TestDeepCopyComplexPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new ComplexPropertiesMock(10);
            var clone = cloner.DeepCopy(original);
            this.AssertIsDeepCopy(original, clone);
        }

        [Fact]
        public void TestShallowCopyComplexPropertiesMock()
        {
            var cloner = this.GetCloner();
            var original = new ComplexPropertiesMock(10);
            var clone = cloner.ShallowCopy(original);
            this.AssertIsShallowCopy(original, clone);
        }

        [Fact]
        public void TestDeepCopyDirectRecursingMock()
        {
            var cloner = this.GetCloner();
            var original = new SelfReferencingMock();
            original.Other = original;
            var clone = cloner.DeepCopy<SelfReferencingMock>(original);
            clone.Should().NotBeSameAs(original, "the original should be deep cloned");
            clone.Other.Should().BeSameAs(clone, "the clone should be referencing itself after the deep copy");
        }

        [Fact]
        public void TestDeepCopyIndirectRecursingMock()
        {
            var cloner = this.GetCloner();
            var original = new SelfReferencingMock();
            original.Other = new SelfReferencingMock { Other = original };
            var clone = cloner.DeepCopy<SelfReferencingMock>(original);
            clone.Should().NotBeSameAs(original, "the original should be deep cloned");
            clone.Other.Should().NotBeSameAs(original.Other,
                "the original's referenced object and clone's referenced object should not be the same object");
            clone.Other.Other.Should().BeSameAs(clone, "the clone's reference should be referencing the clone");
        }

        [Fact]
        public void TestPerformanceDeepCopy()
        {
            Action action = this.TestPerformanceDeepCopyInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }

        [Fact]
        public void TestPerformanceShallowCopy()
        {
            Action action = this.TestPerformanceShallowCopyInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion

        #region Private methods
        private ICloner GetCloner()
        {
            return this.app.GetCloner();
        }

        private void AssertIsDeepCopy(object original, object copy)
        {
            if (original == null)
            {
                copy.Should().BeNull("the copy should be null when the original is null");
                return;
            }

            copy.Should().NotBeNull("the copy should not be null when the original is not");

            var type = original.GetType();
            copy.GetType().Should().Be(type, "the type of the copy should be the same as the original");

            var typeInfo = type.GetTypeInfo();
            if ((typeInfo.IsValueType && typeInfo.IsPrimitive) || typeInfo.IsEnum || type == typeof(string))
            {
                copy.Should().Be(original, "the value of the copy should be the same as the original");
                if (typeInfo.IsPrimitive || type == typeof(string))
                    return;
            }
            else if (!typeInfo.IsValueType)
            {
                copy.Should().NotBeSameAs(original, "the copied object should not be the same instance as the original");
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
                copiedEnumerator.MoveNext().Should().Be(moveNext,
                    "original enumeration elements are {0}, copied enumeration elements are {1}",
                    ((IEnumerable)original).Cast<object>().Count(),
                    ((IEnumerable)copy).Cast<object>().Count());
                while (moveNext)
                {
                    this.AssertIsDeepCopy(originalEnumerator.Current, copiedEnumerator.Current);
                    moveNext = originalEnumerator.MoveNext();
                    copiedEnumerator.MoveNext().Should().Be(moveNext,
                        "original enumeration elements are {0}, copied enumeration elements are {1}",
                        ((IEnumerable)original).Cast<object>().Count(),
                        ((IEnumerable)copy).Cast<object>().Count());
                }
            }
        }

        private void AssertIsShallowCopy(object original, object copy, bool isInitialObject = true)
        {
            if (original == null)
            {
                copy.Should().BeNull("the copy should be null when the original is null");
                return;
            }

            copy.Should().NotBeNull("the copy should not be null when the original is not");

            var type = original.GetType();
            copy.GetType().Should().Be(type, "the type of the copy should be the same as the original");

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsValueType || type == typeof(string))
            {
                copy.Should().Be(original, "the value of the copy should be the same as the original");
                if (typeInfo.IsPrimitive || type == typeof(string))
                    return;
            }
            else if (!isInitialObject)
            {
                copy.Should().BeSameAs(original, "the copied object should be the same instance as the original");
            }

            foreach (var field in this.GetFields(type))
            {
                var originalField = field.GetValue(original);
                var copiedField = field.GetValue(copy);
                this.AssertIsShallowCopy(field.GetValue(original), field.GetValue(copy), false);
            }
        }

        private IEnumerable<FieldInfo> GetFields(Type type)
        {
            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                var fields = baseType.GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in fields)
                {
                    yield return field;
                }
                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        private void TestPerformanceDeepCopyInternal()
        {
            var cloner = this.GetCloner();
            var original = new ComplexPropertiesMock(10);
            for (int i = 0; i < 25; i++)
            {
                cloner.DeepCopy(original);
            }
        }

        private void TestPerformanceShallowCopyInternal()
        {
            var cloner = this.GetCloner();
            var original = new ComplexPropertiesMock(10);
            for (int i = 0; i < 100000; i++)
            {
                cloner.ShallowCopy(original);
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
