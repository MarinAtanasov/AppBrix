﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Tests;
using AppBrix.Web.Client;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Web.Server.Tests;

public sealed class WebClientTests : TestsBase
{
    #region Setup and cleanup
    public WebClientTests() : base(TestUtils.CreateTestApp<WebClientModule>()) => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullHeader()
    {
        var request = this.app.GetFactoryService().GetHttpRequest();
        var action = () => request.SetHeader(string.Empty);
        action.Should().Throw<ArgumentNullException>("header cannot be empty");
    }
    
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullMethod()
    {
        var request = this.app.GetFactoryService().GetHttpRequest();
        var action = () => request.SetMethod(string.Empty);
        action.Should().Throw<ArgumentNullException>("method cannot be empty");
    }
    
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullUrl()
    {
        var request = this.app.GetFactoryService().GetHttpRequest();
        var action = () => request.SetUrl(string.Empty);
        action.Should().Throw<ArgumentNullException>("url cannot be empty");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetHttpRequest() => TestUtils.TestPerformance(this.TestPerformanceGetHttpRequestInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetHttpRequestInternal()
    {
        var service = this.app.GetFactoryService();
        for (var i = 0; i < 10000; i++)
        {
            service.GetHttpRequest();
        }
    }
    #endregion
}