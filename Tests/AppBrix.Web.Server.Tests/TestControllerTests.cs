﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Tests;
using AppBrix.Web.Client;
using AppBrix.Web.Client.Contracts;
using AppBrix.Web.Server.Events;
using AppBrix.Web.Server.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using Xunit;

namespace AppBrix.Web.Server.Tests;

public sealed class TestControllerTests
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async void TestConnection()
    {
        var app = this.CreateApp();
        using var webApp = this.CreateTestWebApp(app);
        using var client = webApp.GetTestClient();
        app.Container.Register(client);

        var response = await app.GetFactoryService().GetHttpRequest()
            .SetUrl(TestControllerTests.TestConnectionServiceUrl)
            .SetHeader("x-test", "test")
            .SetHeader("x-test")
            .Send()
            .ConfigureAwait(false);
        response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the GET request should return status OK");

        var postResponse = await app.GetFactoryService().GetHttpRequest()
            .SetUrl(TestControllerTests.TestConnectionServiceUrl)
            .SetClientName(string.Empty)
            .SetContent(42)
            .SetHeader("Content-Type", "application/json")
            .SetMethod(HttpMethod.Post)
            .SetVersion(new Version(1, 1))
            .Send<int>()
            .ConfigureAwait(false);
        postResponse.StatusCode.Should().Be((int)HttpStatusCode.OK, "the POST request should return status OK");
        postResponse.Content.Should().Be(42, "the request should return the same integer that has been passed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async void TestConnectionBetweenTwoApps()
    {
        var app1 = this.CreateApp();
        var app2 = this.CreateApp();

        using var webApp1 = this.CreateTestWebApp(app1);
        using var app1Client = webApp1.GetTestClient();
        using var webApp2 = this.CreateTestWebApp(app2);
        using var app2Client = webApp2.GetTestClient();

        app1.Container.Register(app2Client);
        var response1 = await app1.GetFactoryService().GetHttpRequest()
            .SetUrl(TestControllerTests.AppIdService2Url)
            .Send<AppIdMessage>()
            .ConfigureAwait(false);
        response1.StatusCode.Should().Be((int)HttpStatusCode.OK, "the first app's call should reach the second app's service");
        response1.Content.Id.Should().Be(app2.ConfigService.Get<AppIdConfig>().Id, "the first app should receive the second app's id");

        app2.Container.Register(app1Client);
        var response2 = await app2.GetFactoryService().GetHttpRequest()
            .SetUrl(TestControllerTests.AppIdServiceUrl)
            .Send<AppIdMessage>()
            .ConfigureAwait(false);
        response2.StatusCode.Should().Be((int)HttpStatusCode.OK, "the second app's call should reach the first app's service");
        response2.Content.Id.Should().Be(app1.ConfigService.Get<AppIdConfig>().Id, "the second app should receive the first app's id");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceWebServer()
    {
        var app = this.CreateApp();
        using var webApp = this.CreateTestWebApp(app);
        using var client = webApp.GetTestClient();
        app.Container.Register(client);
        TestUtils.TestPerformance(() => this.TestPerformanceWebServerInternal(app));
    }
    #endregion

    #region Private methods
    private WebApplication CreateTestWebApp(IApp app)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = this.GetType().Assembly.GetName().Name
        });
        builder.WebHost.UseTestServer();
        var webApp = builder.Build(app);
        webApp.Start();
        return webApp;
    }

    private IApp CreateApp()
    {
        var app = TestUtils.CreateTestApp<WebServerModule, WebClientModule>();
        app.ConfigService.Get<AppIdConfig>().Id = Guid.NewGuid();
        app.Start();
        app.GetEventHub().Subscribe<IConfigureWebAppBuilder>(this.ConfigureWebAppBuilder);
        app.GetEventHub().Subscribe<IConfigureWebApp>(this.ConfigureWebApp);
        return app;
    }

    private void ConfigureWebAppBuilder(IConfigureWebAppBuilder args)
    {
        args.Builder.Services.AddControllers();
    }

    private void ConfigureWebApp(IConfigureWebApp args)
    {
        var app = args.App;
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }

    private void TestPerformanceWebServerInternal(IApp app)
    {
        for (var i = 0; i < 100; i++)
        {
            app.GetFactoryService()
                .GetHttpRequest()
                .SetUrl(TestControllerTests.TestConnectionServiceUrl)
                .Send<string>()
                .GetAwaiter()
                .GetResult();
        }
    }
    #endregion

    #region Private fields and constants
    private const string ServerBaseAddress = "http://localhost:1337/";
    private const string TestConnectionServiceUrl = TestControllerTests.ServerBaseAddress + "api/testconnection";
    private const string AppIdServiceUrl = TestControllerTests.ServerBaseAddress + "api/appid";

    private const string Server2BaseAddress = "http://localhost:1338/";
    private const string AppIdService2Url = TestControllerTests.Server2BaseAddress + "api/appid";
    #endregion
}
