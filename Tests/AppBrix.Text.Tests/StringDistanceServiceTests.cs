// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
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
        var action = () => this.App.GetStringDistanceService().GetDamerauLevenshteinDistance(null!, "appbrix");
        action.Should().Throw<ArgumentNullException>("left should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceNullRight()
    {
        var action = () => this.App.GetStringDistanceService().GetDamerauLevenshteinDistance("appbrix", null!);
        action.Should().Throw<ArgumentNullException>("right should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEmptyLeft()
    {
        this.App.GetStringDistanceService()
            .GetDamerauLevenshteinDistance(string.Empty, "appbrix")
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEmptyRight()
    {
        this.App.GetStringDistanceService()
            .GetDamerauLevenshteinDistance("appbrix", string.Empty)
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEqualStrings()
    {
        this.App.GetStringDistanceService()
            .GetDamerauLevenshteinDistance("appbrix", "appbrix")
            .Should()
            .Be(0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistance()
    {
        this.App.GetStringDistanceService()
            .GetDamerauLevenshteinDistance("appbrix", "apprtbix")
            .Should()
            .Be(2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceMixedMinMaxCharacters()
    {
        this.App.GetStringDistanceService()
            .GetDamerauLevenshteinDistance("cbd", "cae")
            .Should()
            .Be(2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetDamerauLevenshteinDistance() => this.AssertPerformance(this.TestPerformanceGetDamerauLevenshteinDistanceInternal);
    #endregion

    #region Test Levenshtein distance
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceNullLeft()
    {
        var action = () => this.App.GetStringDistanceService().GetLevenshteinDistance(null!, "appbrix");
        action.Should().Throw<ArgumentNullException>("left should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceNullRight()
    {
        var action = () => this.App.GetStringDistanceService().GetLevenshteinDistance("appbrix", null!);
        action.Should().Throw<ArgumentNullException>("right should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEmptyLeft()
    {
        this.App.GetStringDistanceService()
            .GetLevenshteinDistance(string.Empty, "appbrix")
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEmptyRight()
    {
        this.App.GetStringDistanceService()
            .GetLevenshteinDistance("appbrix", string.Empty)
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEqualStrings()
    {
        this.App.GetStringDistanceService()
            .GetLevenshteinDistance("appbrix", "appbrix")
            .Should()
            .Be(0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistance()
    {
        this.App.GetStringDistanceService()
            .GetLevenshteinDistance("appbrix", "apbtix")
            .Should()
            .Be(2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetLevenshteinDistance() => this.AssertPerformance(this.TestPerformanceGetLevenshteinDistanceInternal);
    #endregion

    #region Test Optimal String Alignment distance
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceNullLeft()
    {
        var action = () => this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance(null!, "appbrix");
        action.Should().Throw<ArgumentNullException>("left should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceNullRight()
    {
        var action = () => this.App.GetStringDistanceService().GetOptimalStringAlignmentDistance("appbrix", null!);
        action.Should().Throw<ArgumentNullException>("right should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEmptyLeft()
    {
        this.App.GetStringDistanceService()
            .GetOptimalStringAlignmentDistance(string.Empty, "appbrix")
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEmptyRight()
    {
        this.App.GetStringDistanceService()
            .GetOptimalStringAlignmentDistance("appbrix", string.Empty)
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEqualStrings()
    {
        this.App.GetStringDistanceService()
            .GetOptimalStringAlignmentDistance("appbrix", "appbrix")
            .Should()
            .Be(0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistance()
    {
        this.App.GetStringDistanceService()
            .GetOptimalStringAlignmentDistance("appbrix", "aprbix")
            .Should()
            .Be(2);
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
