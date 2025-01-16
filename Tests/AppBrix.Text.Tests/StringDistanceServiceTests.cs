// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using Xunit;

namespace AppBrix.Text.Tests;

public sealed class StringDistanceServiceTests : TestsBase<TextModule>
{
    #region Setup and cleanup
    public StringDistanceServiceTests() => this.App.Start();
    #endregion

    #region Tests Damerau-Levenshtein distance
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceNullLeft()
    {
        Action action = () => this.App.GetStringDistanceService().GetDamerauLevenshteinDistance(null!, "appbrix");
        this.AssertThrows<ArgumentNullException>(action, "left should not be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceNullRight()
    {
        Action action = () => this.App.GetStringDistanceService().GetDamerauLevenshteinDistance("appbrix", null!);
        this.AssertThrows<ArgumentNullException>(action, "right should not be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEmptyLeft()
    {
        this.Assert(this.App.GetStringDistanceService().GetDamerauLevenshteinDistance(string.Empty, "appbrix") == 7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEmptyRight()
    {
        this.Assert(this.App.GetStringDistanceService().GetDamerauLevenshteinDistance("appbrix", string.Empty) == 7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEqualStrings()
    {
        this.Assert(this.App.GetStringDistanceService().GetDamerauLevenshteinDistance("appbrix", "appbrix") == 0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistance()
    {
        this.Assert(this.App.GetStringDistanceService().GetDamerauLevenshteinDistance("appbrix", "apprtbix") == 2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceMixedMinMaxCharacters()
    {
        this.Assert(this.App.GetStringDistanceService().GetDamerauLevenshteinDistance("cbd", "cae") == 2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetDamerauLevenshteinDistance() => this.AssertPerformance(this.TestPerformanceGetDamerauLevenshteinDistanceInternal);
    #endregion

    #region Test Levenshtein distance
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceNullLeft()
    {
        Action action = () => this.App.GetStringDistanceService().GetLevenshteinDistance(null!, "appbrix");
        this.AssertThrows<ArgumentNullException>(action, "left should not be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceNullRight()
    {
        Action action = () => this.App.GetStringDistanceService().GetLevenshteinDistance("appbrix", null!);
        this.AssertThrows<ArgumentNullException>(action, "right should not be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEmptyLeft()
    {
        this.Assert(this.App.GetStringDistanceService().GetLevenshteinDistance(string.Empty, "appbrix") == 7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEmptyRight()
    {
        this.Assert(this.App.GetStringDistanceService().GetLevenshteinDistance("appbrix", string.Empty) == 7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEqualStrings()
    {
        this.Assert(this.App.GetStringDistanceService().GetLevenshteinDistance("appbrix", "appbrix") == 0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistance()
    {
        this.Assert(this.App.GetStringDistanceService().GetLevenshteinDistance("appbrix", "apbtix")  == 2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetLevenshteinDistance() => this.AssertPerformance(this.TestPerformanceGetLevenshteinDistanceInternal);
    #endregion

    #region Test Optimal String Alignment distance
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceNullLeft()
    {
        Action action = () => this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance(null!, "appbrix");
        this.AssertThrows<ArgumentNullException>(action, "left should not be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceNullRight()
    {
        Action action = () => this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance("appbrix", null!);
        this.AssertThrows<ArgumentNullException>(action, "right should not be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEmptyLeft()
    {
        this.Assert(this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance(string.Empty, "appbrix") == 7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEmptyRight()
    {
        this.Assert(this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance("appbrix", string.Empty) == 7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEqualStrings()
    {
        this.Assert(this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance("appbrix", "appbrix") == 0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistance()
    {
        this.Assert(this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance("appbrix", "aprbix") == 2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetOptimalStringAlignmentDistance() => this.AssertPerformance(this.TestPerformanceGetOptimalStringAlignmentDistanceInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetDamerauLevenshteinDistanceInternal()
    {
        var service = this.App.GetStringDistanceService();
        for (var i = 0; i < 12500; i++)
        {
            service.GetDamerauLevenshteinDistance("AppBrix", "Framework");
        }
    }

    private void TestPerformanceGetLevenshteinDistanceInternal()
    {
        var service = this.App.GetStringDistanceService();
        for (var i = 0; i < 30000; i++)
        {
            service.GetLevenshteinDistance("AppBrix", "Framework");
        }
    }

    private void TestPerformanceGetOptimalStringAlignmentDistanceInternal()
    {
        var service = this.App.GetStringDistanceService();
        for (var i = 0; i < 20000; i++)
        {
            service.GetOptimalStringAlignmentDistance("AppBrix", "Framework");
        }
    }
    #endregion
}
