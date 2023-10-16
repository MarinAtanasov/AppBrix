// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Text.Tests;

public sealed class StringDistanceServiceTests : TestsBase<TextModule>
{
    #region Setup and cleanup
    public StringDistanceServiceTests() => this.app.Start();
    #endregion

    #region Tests Damerau-Levenshtein distance
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceNullLeft()
    {
        var action = () => this.app.GetStringDistanceService().GetDamerauLevenshteinDistance(null!, "appbrix");
        action.Should().Throw<ArgumentNullException>("left should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceNullRight()
    {
        var action = () => this.app.GetStringDistanceService().GetDamerauLevenshteinDistance("appbrix", null!);
        action.Should().Throw<ArgumentNullException>("right should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEmptyLeft()
    {
        this.app.GetStringDistanceService()
            .GetDamerauLevenshteinDistance(string.Empty, "appbrix")
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEmptyRight()
    {
        this.app.GetStringDistanceService()
            .GetDamerauLevenshteinDistance("appbrix", string.Empty)
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceEqualStrings()
    {
        this.app.GetStringDistanceService()
            .GetDamerauLevenshteinDistance("appbrix", "appbrix")
            .Should()
            .Be(0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistance()
    {
        this.app.GetStringDistanceService()
            .GetDamerauLevenshteinDistance("appbrix", "apprtbix")
            .Should()
            .Be(2);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDamerauLevenshteinDistanceMixedMinMaxCharacters()
    {
        this.app.GetStringDistanceService()
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
        var action = () => this.app.GetStringDistanceService().GetLevenshteinDistance(null!, "appbrix");
        action.Should().Throw<ArgumentNullException>("left should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceNullRight()
    {
        var action = () => this.app.GetStringDistanceService().GetLevenshteinDistance("appbrix", null!);
        action.Should().Throw<ArgumentNullException>("right should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEmptyLeft()
    {
        this.app.GetStringDistanceService()
            .GetLevenshteinDistance(string.Empty, "appbrix")
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEmptyRight()
    {
        this.app.GetStringDistanceService()
            .GetLevenshteinDistance("appbrix", string.Empty)
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistanceEqualStrings()
    {
        this.app.GetStringDistanceService()
            .GetLevenshteinDistance("appbrix", "appbrix")
            .Should()
            .Be(0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetLevenshteinDistance()
    {
        this.app.GetStringDistanceService()
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
        var action = () => this.app.GetStringDistanceService().GetOptimalStringAlignmentDistance(null!, "appbrix");
        action.Should().Throw<ArgumentNullException>("left should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceNullRight()
    {
        var action = () => this.app.GetStringDistanceService().GetOptimalStringAlignmentDistance("appbrix", null!);
        action.Should().Throw<ArgumentNullException>("right should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEmptyLeft()
    {
        this.app.GetStringDistanceService()
            .GetOptimalStringAlignmentDistance(string.Empty, "appbrix")
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEmptyRight()
    {
        this.app.GetStringDistanceService()
            .GetOptimalStringAlignmentDistance("appbrix", string.Empty)
            .Should()
            .Be(7);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistanceEqualStrings()
    {
        this.app.GetStringDistanceService()
            .GetOptimalStringAlignmentDistance("appbrix", "appbrix")
            .Should()
            .Be(0);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetOptimalStringAlignmentDistance()
    {
        this.app.GetStringDistanceService()
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
        var service = this.app.GetStringDistanceService();
        for (var i = 0; i < 12500; i++)
        {
            service.GetDamerauLevenshteinDistance("AppBrix", "Framework");
        }
    }

    private void TestPerformanceGetLevenshteinDistanceInternal()
    {
        var service = this.app.GetStringDistanceService();
        for (var i = 0; i < 30000; i++)
        {
            service.GetLevenshteinDistance("AppBrix", "Framework");
        }
    }

    private void TestPerformanceGetOptimalStringAlignmentDistanceInternal()
    {
        var service = this.app.GetStringDistanceService();
        for (var i = 0; i < 20000; i++)
        {
            service.GetOptimalStringAlignmentDistance("AppBrix", "Framework");
        }
    }
    #endregion
}
