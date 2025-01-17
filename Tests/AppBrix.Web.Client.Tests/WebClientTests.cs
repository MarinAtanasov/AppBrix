// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using System;

namespace AppBrix.Web.Client.Tests;

[TestClass]
public sealed class WebClientTests : TestsBase<WebClientModule>
{
    #region Setup and cleanup
    protected override void Initialize() => this.App.Start();
    #endregion

    #region Tests
    [Test, Functional]
    public void TestSetNullHeader()
    {
        var request = this.App.GetFactoryService().GetHttpRequest();
        var action = () => request.SetHeader(string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "header cannot be empty");;
    }

    [Test, Functional]
    public void TestSetNullMethod()
    {
        var request = this.App.GetFactoryService().GetHttpRequest();
        var action = () => request.SetMethod(string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "method cannot be empty");;
    }

    [Test, Functional]
    public void TestSetNullUrl()
    {
        var request = this.App.GetFactoryService().GetHttpRequest();
        var action = () => request.SetUrl(string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "url cannot be empty");;
    }

    [Test, Performance]
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
