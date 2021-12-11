// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Cloning.Services;
using AppBrix.Cloning.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AppBrix.Cloning.Tests;

public sealed class CloningTests : TestsBase
{
    #region Setup and cleanup
    public CloningTests() : base(TestUtils.CreateTestApp<CloningModule>()) => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeepCopyInteger()
    {
        var cloner = this.GetCloner();
        var original = 5;
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShallowCopyInteger()
    {
        var cloner = this.GetCloner();
        var original = 5;
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeepCopyString()
    {
        var cloner = this.GetCloner();
        var original = "Test";
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShallowCopyString()
    {
        var cloner = this.GetCloner();
        var original = "Test";
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeepCopyNumericPropertiesMock()
    {
        var cloner = this.GetCloner();
        var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShallowCopyNumericPropertiesMock()
    {
        var cloner = this.GetCloner();
        var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeepCopyPrimitivePropertiesMock()
    {
        var cloner = this.GetCloner();
        var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now, TimeSpan.FromMilliseconds(42));
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShallowCopyPrimitivePropertiesMock()
    {
        var cloner = this.GetCloner();
        var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now, TimeSpan.FromMilliseconds(42));
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeepCopyComplexPropertiesMock()
    {
        var cloner = this.GetCloner();
        var original = new ComplexPropertiesMock(10);
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShallowCopyComplexPropertiesMock()
    {
        var cloner = this.GetCloner();
        var original = new ComplexPropertiesMock(10);
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeepCopyDirectRecursingMock()
    {
        var cloner = this.GetCloner();
        var original = new SelfReferencingMock();
        original.Other = original;
        var clone = cloner.DeepCopy<SelfReferencingMock>(original);
        clone.Should().NotBeSameAs(original, "the original should be deep cloned");
        clone.Other.Should().BeSameAs(clone, "the clone should be referencing itself after the deep copy");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeepCopyDelegateMock()
    {
        var cloner = this.GetCloner();
        var original = new DelegateWrapperMock();
        original.Delegate = () => original;
        var clone = cloner.DeepCopy<DelegateWrapperMock>(original);
        original.Delegate().Should().BeSameAs(original, "the original's delegate should not be changed");
        clone.Should().NotBeSameAs(original, "the original should be deep cloned");
        clone.Delegate.Should().NotBeNull("the delegate should be cloned");
        clone.Delegate().Should().BeSameAs(clone, "the delegated reference to the original object should be switched to the cloned one");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceDeepCopy() => TestUtils.TestPerformance(this.TestPerformanceDeepCopyInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceShallowCopy() => TestUtils.TestPerformance(this.TestPerformanceShallowCopyInternal);
    #endregion

    #region Private methods
    private ICloner GetCloner() => this.app.GetCloner();

    private void AssertIsDeepCopy(object original, object copy, string property = "this")
    {
        if (original is null)
        {
            copy.Should().BeNull($"{property}'s copy should be null when the original is null");
            return;
        }

        copy.Should().NotBeNull($"{property}'s copy should not be null when the original is not");

        var type = original.GetType();
        copy.GetType().Should().Be(type, $"{property}'s type of the copy should be the same as the original");

        if ((type.IsValueType && type.IsPrimitive) || type.IsEnum || type == typeof(string))
        {
            copy.Should().Be(original, $"{property}'s value of the copy should be the same as the original");
            if (type.IsPrimitive || type == typeof(string))
                return;
        }
        else if (!type.IsValueType)
        {
            copy.Should().NotBeSameAs(original, $"{property}'s copied object should not be the same instance as the original");
        }

        foreach (var field in this.GetFields(type))
        {
            this.AssertIsDeepCopy(field.GetValue(original), field.GetValue(copy), $"{property}.{field.Name}");
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            var originalEnumerator = ((IEnumerable)original).GetEnumerator();
            var copiedEnumerator = ((IEnumerable)copy).GetEnumerator();

            var index = -1;
            while (true)
            {
                var moveNext = originalEnumerator.MoveNext();
                copiedEnumerator.MoveNext().Should().Be(moveNext,
                    $"{property}'s original enumeration elements are {{0}}, copied enumeration elements are {{1}}",
                    ((IEnumerable)original).Cast<object>().Count(),
                    ((IEnumerable)copy).Cast<object>().Count());
                index++;

                if (!moveNext)
                    break;

                this.AssertIsDeepCopy(originalEnumerator.Current, copiedEnumerator.Current, $"{property}[{index}]");
            }
        }
    }

    private void AssertIsShallowCopy(object original, object copy, bool isInitialObject = true)
    {
        if (original is null)
        {
            copy.Should().BeNull("the copy should be null when the original is null");
            return;
        }

        copy.Should().NotBeNull("the copy should not be null when the original is not");

        var type = original.GetType();
        copy.GetType().Should().Be(type, "the type of the copy should be the same as the original");

        if (type.IsValueType || type == typeof(string))
        {
            copy.Should().Be(original, "the value of the copy should be the same as the original");
            if (type.IsPrimitive || type == typeof(string))
                return;
        }
        else if (!isInitialObject)
        {
            copy.Should().BeSameAs(original, "the copied object should be the same instance as the original");
        }

        foreach (var field in this.GetFields(type))
        {
            this.AssertIsShallowCopy(field.GetValue(original), field.GetValue(copy), false);
        }
    }

    private IEnumerable<FieldInfo> GetFields(Type type)
    {
        for (var baseType = type; baseType != typeof(object) && baseType is not null; baseType = baseType.BaseType)
        {
            var fields = baseType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);
            for (var i = 0; i < fields.Length; i++)
            {
                yield return fields[i];
            }
        }
    }

    private void TestPerformanceDeepCopyInternal()
    {
        var cloner = this.GetCloner();
        var original = new ComplexPropertiesMock(10);
        for (var i = 0; i < 200; i++)
        {
            cloner.DeepCopy(original);
        }
    }

    private void TestPerformanceShallowCopyInternal()
    {
        var cloner = this.GetCloner();
        var original = new ComplexPropertiesMock(10);
        for (var i = 0; i < 100000; i++)
        {
            cloner.ShallowCopy(original);
        }
    }
    #endregion
}
