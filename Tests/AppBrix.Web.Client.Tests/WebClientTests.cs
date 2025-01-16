// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using Xunit;

namespace AppBrix.Web.Client.Tests;

public sealed class WebClientTests : TestsBase<WebClientModule>
{
    #region Setup and cleanup
    public WebClientTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullHeader()
    {
        var request = this.App.GetFactoryService().GetHttpRequest();
        var action = () => request.SetHeader(string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "header cannot be empty");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullMethod()
    {
        var request = this.App.GetFactoryService().GetHttpRequest();
        var action = () => request.SetMethod(string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "method cannot be empty");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullUrl()
    {
        var request = this.App.GetFactoryService().GetHttpRequest();
        var action = () => request.SetUrl(string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "url cannot be empty");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetHttpRequest() => this.AssertPerformance(this.TestPerformanceGetHttpRequestInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetHttpRequestInternal()
    {
        var service = this.App.GetFactoryService();
        for (var i = 0; i < 150000; i++)
        {
            service.GetHttpRequest();
        }
    }
    #endregion
}
