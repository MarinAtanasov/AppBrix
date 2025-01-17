// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Cloning.Tests.Mocks;
using AppBrix.Testing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Cloning.Tests;

[TestClass]
public sealed class CloningTests : TestsBase<CloningModule>
{
    #region Test lifecycle
    protected override void Initialize() => this.App.Start();
    #endregion

    #region Deep Copy Tests
    [Test, Functional]
    public void TestDeepCopyNull()
    {
        var cloner = this.App.GetCloningService();
        var action = () => cloner.DeepCopy<object>(null);
        this.AssertThrows<ArgumentNullException>(action, "parameter cannot be null");
    }

    [Test, Functional]
    public void TestDeepCopyType()
    {
        var type = typeof(CloningTests);
        this.Assert(this.App.GetCloningService().DeepCopy(type) == type, "deep copy of type is the same type");
    }

    [Test, Functional]
    public void TestDeepCopyInteger()
    {
        var cloner = this.App.GetCloningService();
        var original = 5;
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Test, Functional]
    public void TestDeepCopyString()
    {
        var cloner = this.App.GetCloningService();
        var original = "Test";
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Test, Functional]
    public void TestDeepCopyEmptyArray()
    {
        var cloner = this.App.GetCloningService();
        var original = Array.Empty<int>();
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Test, Functional]
    public void TestDeepCopyArray()
    {
        var cloner = this.App.GetCloningService();
        var original = new[,] { {1, 10}, {2, 20}, {3, 30} };
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Test, Functional]
    public void TestDeepCopyNumericPropertiesMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Test, Functional]
    public void TestDeepCopyPrimitivePropertiesMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now, TimeSpan.FromMilliseconds(42));
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Test, Functional]
    public void TestDeepCopyComplexPropertiesMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new ComplexPropertiesMock(10);
        var clone = cloner.DeepCopy(original);
        this.AssertIsDeepCopy(original, clone);
    }

    [Test, Functional]
    public void TestDeepCopyDirectRecursingMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new SelfReferencingMock();
        original.Other = original;
        var clone = cloner.DeepCopy(original);
        this.Assert(clone != original, "the original should be deep cloned");
        this.Assert(clone.Other == clone, "the clone should be referencing itself after the deep copy");
    }

    [Test, Functional]
    public void TestDeepCopyIndirectRecursingMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new SelfReferencingMock();
        original.Other = new SelfReferencingMock { Other = original };
        var clone = cloner.DeepCopy(original);
        this.Assert(clone != original, "the original should be deep cloned");
        this.Assert(clone.Other != original.Other, "the original's referenced object and clone's referenced object should not be the same object");
        this.Assert(clone.Other.Other == clone, "the clone's reference should be referencing the clone");
    }

    [Test, Functional]
    public void TestDeepCopyDelegateMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new DelegateWrapperMock();
        original.Delegate = () => original;
        var clone = cloner.DeepCopy(original);
        this.Assert(original.Delegate() == original, "the original's delegate should not be changed");
        this.Assert(clone != original, "the original should be deep cloned");
        this.Assert(clone.Delegate is not null, "the delegate should be cloned");
        this.Assert(clone.Delegate!() == clone, "the delegated reference to the original object should be switched to the cloned one");
    }

    [Test, Performance]
    public void TestPerformanceDeepCopy() => this.AssertPerformance(this.TestPerformanceDeepCopyInternal);
    #endregion

    #region Shallow Copy Tests
    [Test, Functional]
    public void TestShallowCopyNull()
    {
        var cloner = this.App.GetCloningService();
        var action = () => cloner.ShallowCopy<object>(null);
        this.AssertThrows<ArgumentNullException>(action, "parameter cannot be null");
    }

    [Test, Functional]
    public void TestShallowCopyType()
    {
        var type = typeof(CloningTests);
        this.Assert(this.App.GetCloningService().ShallowCopy(type).ToString() == type.ToString(), "shallow copy of type is the same type");
    }

    [Test, Functional]
    public void TestShallowCopyInteger()
    {
        const int original = 5;
        var cloner = this.App.GetCloningService();
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Test, Functional]
    public void TestShallowCopyString()
    {
        var original = "Test";
        var cloner = this.App.GetCloningService();
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Test, Functional]
    public void TestShallowCopyEmptyArray()
    {
        var cloner = this.App.GetCloningService();
        var original = Array.Empty<int>();
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Test, Functional]
    public void TestShallowCopyArray()
    {
        var cloner = this.App.GetCloningService();
        var original = new[,] { {1, 10}, {2, 20}, {3, 30} };
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Test, Functional]
    public void TestShallowCopyNumericPropertiesMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new NumericPropertiesMock(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7);
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Test, Functional]
    public void TestShallowCopyPrimitivePropertiesMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new PrimitivePropertiesMock(true, 't', "Test", DateTime.Now, TimeSpan.FromMilliseconds(42));
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Test, Functional]
    public void TestShallowCopyComplexPropertiesMock()
    {
        var cloner = this.App.GetCloningService();
        var original = new ComplexPropertiesMock(10);
        var clone = cloner.ShallowCopy(original);
        this.AssertIsShallowCopy(original, clone);
    }

    [Test, Performance]
    public void TestPerformanceShallowCopy() => this.AssertPerformance(this.TestPerformanceShallowCopyInternal);
    #endregion

    #region Private methods
    private void AssertIsDeepCopy(object original, object copy, string property = "this")
    {
        if (original is null)
        {
            this.Assert(copy is null, $"{property}'s copy should be null when the original is null");
            return;
        }

        this.Assert(copy is not null, $"{property}'s copy should not be null when the original is not");

        var type = original.GetType();
        this.Assert(copy!.GetType() == type, $"{property}'s type of the copy should be the same as the original");

        if (type.IsValueType && type.IsPrimitive || type.IsEnum || type == typeof(string))
        {
            this.Assert(copy.Equals(original), $"{property}'s value of the copy should be the same as the original");
            if (type.IsPrimitive || type == typeof(string))
                return;
        }
        else if (!type.IsValueType)
        {
            this.Assert(copy != original, $"{property}'s copied object should not be the same instance as the original");
        }

        foreach (var field in this.GetFields(type))
        {
            this.AssertIsDeepCopy(field.GetValue(original), field.GetValue(copy), $"{property}.{field.Name}");
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            var originalEnumerator = ((IEnumerable)original).GetEnumerator();
            using var originalEnumeratorDisposable = originalEnumerator as IDisposable;
            var copiedEnumerator = ((IEnumerable)copy).GetEnumerator();
            using var copiedEnumeratorDisposable = copiedEnumerator as IDisposable;

            var index = -1;
            while (true)
            {
                var moveNext = originalEnumerator.MoveNext();
                this.Assert(copiedEnumerator.MoveNext() == moveNext,
                    $"{property}'s original enumeration elements are {((IEnumerable)original).Cast<object>().Count()}, copied enumeration elements are {((IEnumerable)copy).Cast<object>().Count()}");

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
            this.Assert(copy is null, "the copy should be null when the original is null");
            return;
        }

        this.Assert(copy is not null, "the copy should not be null when the original is not");

        var type = original.GetType();
        this.Assert(copy!.GetType() == type, "the type of the copy should be the same as the original");

        if (type.IsValueType || type == typeof(string))
        {
            this.Assert(copy.Equals(original), "the value of the copy should be the same as the original");
            if (type.IsPrimitive || type == typeof(string))
                return;
        }
        else if (!isInitialObject)
        {
            this.Assert(copy == original, "the copied object should be the same instance as the original");
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
            foreach (var field in baseType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public))
            {
                yield return field;
            }
        }
    }

    private void TestPerformanceDeepCopyInternal()
    {
        var cloner = this.App.GetCloningService();
        var original = new ComplexPropertiesMock(10);
        for (var i = 0; i < 200; i++)
        {
            cloner.DeepCopy(original);
        }
    }

    private void TestPerformanceShallowCopyInternal()
    {
        var cloner = this.App.GetCloningService();
        var original = new ComplexPropertiesMock(10);
        for (var i = 0; i < 200000; i++)
        {
            cloner.ShallowCopy(original);
        }
    }
    #endregion
}
