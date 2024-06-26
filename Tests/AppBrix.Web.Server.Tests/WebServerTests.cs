﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using AppBrix.Web.Client;
using AppBrix.Web.Client.Contracts;
using AppBrix.Web.Server.Events;
using AppBrix.Web.Server.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace AppBrix.Web.Server.Tests;

public sealed class WebServerTests : TestsBase
{
    #region Setup and cleanup
    public WebServerTests() : base(WebServerTests.CreateApp())
    {
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestConnection()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var response = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.ConnectionTestServiceUrl)
            .SetHeader("x-test", "test")
            .SetHeader("x-test")
            .Send();
        response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the GET request should return status OK");

        var postResponse = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.ConnectionTestServiceUrl)
            .SetClientName(string.Empty)
            .SetContent(42)
            .SetHeader("Content-Type", "application/json")
            .SetMethod(HttpMethod.Post)
            .SetVersion(new Version(1, 1))
            .Send<int>();
        postResponse.StatusCode.Should().Be((int)HttpStatusCode.OK, "the POST request should return status OK");
        postResponse.Content.Should().Be(42, "the request should return the same integer that has been passed");

        await webApp.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestConnectionBetweenTwoApps()
    {
        var app1 = this.App;
        var app2 = WebServerTests.CreateApp();

        await using var webApp1 = this.CreateTestWebApp(app1);
        using var app1Client = webApp1.GetTestClient();
        await using var webApp2 = this.CreateTestWebApp(app2);
        using var app2Client = webApp2.GetTestClient();

        app1.Container.Register(app2Client);
        var response1 = await app1.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.AppIdService2Url)
            .Send<AppIdMessage>();
        response1.StatusCode.Should().Be((int)HttpStatusCode.OK, "the first app's call should reach the second app's service");
        response1.Content.Id.Should().Be(app2.ConfigService.Get<AppIdConfig>().Id, "the first app should receive the second app's id");

        app2.Container.Register(app1Client);
        var response2 = await app2.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.AppIdServiceUrl)
            .Send<AppIdMessage>();
        response2.StatusCode.Should().Be((int)HttpStatusCode.OK, "the second app's call should reach the first app's service");
        response2.Content.Id.Should().Be(app1.ConfigService.Get<AppIdConfig>().Id, "the second app should receive the first app's id");

        await webApp2.StopAsync();
        await webApp1.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestEchoGetString()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var response = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl($"{WebServerTests.EchoServiceUrl}/{nameof(this.TestEchoGetString)}")
            .Send<string>();
        response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the request should return status OK");
        response.ReasonPhrase.Should().Be(HttpStatusCode.OK.ToString(), "the request should return status OK");
        response.Version.Should().Be(new Version(1, 1), "the version of the response should be 1.1");
        response.Content.Should().Be(nameof(this.TestEchoGetString), "the response should echo the request");
        response.Headers.Count.Should().Be(1, "only the content-type header should be returned");
        response.Headers["Content-Type"].Should().Equal(["text/plain; charset=utf-8"], "the content type should be a utf-8 string");

        await webApp.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestEchoPostJson()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var model = new EchoModel
        {
            DateTime = new DateTime(2021, 12, 13, 14, 30, 42, 230, DateTimeKind.Utc),
            TimeSpan = TimeSpan.FromMilliseconds(42),
            Value = 42,
            Version = new Version(1, 0, 1)
        };
        var response = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.EchoServiceUrl)
            .SetContent(model)
            .SetHeader("Content-Type", "application/json")
            .SetMethod(HttpMethod.Post)
            .Send<EchoModel>();
        response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the request should return status OK");
        response.Content.Should().Be(model, "the response should echo the request");

        await webApp.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestEchoPostJsonDefaultModel()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var model = new EchoModel();
        var response = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.EchoServiceUrl)
            .SetContent(model)
            .SetHeader("Content-Type", "application/json")
            .SetExpiresHeader(this.App.GetTime().AddYears(1))
            .SetLastModifiedHeader(this.App.GetTime())
            .SetMethod(HttpMethod.Post)
            .Send<EchoModel>();
        response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the request should return status OK");
        response.Content.Should().Be(model, "the response should echo the request");

        await webApp.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestEchoPostBytes()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var model = new EchoModel();
        var response = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.EchoServiceUrl)
            .SetContent(model)
            .SetHeader("Content-Type", "application/json")
            .SetExpiresHeader(this.App.GetTime().AddYears(1))
            .SetLastModifiedHeader(this.App.GetTime())
            .SetMethod(HttpMethod.Post)
            .Send<byte[]>();
        response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the request should return status OK");
        response.Content.Length.Should().BePositive("the response should echo the request");

        await webApp.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestEchoPostStream()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var model = new EchoModel();
        var request = this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.EchoServiceUrl)
            .SetContent(model)
            .SetHeader("Content-Type", "application/json")
            .SetExpiresHeader(this.App.GetTime().AddYears(1))
            .SetLastModifiedHeader(this.App.GetTime())
            .SetMethod(HttpMethod.Post);
        await using (var response = await request.SendStream())
        {
            response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the request should return status OK");
            response.Content.ReadByte().Should().BePositive("the response stream should not be empty");
        }

        await webApp.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestEchoPostStreamAsyncDisposable()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var model = new EchoModel();
        var request = this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.EchoServiceUrl)
            .SetContent(model)
            .SetHeader("Content-Type", "application/json")
            .SetExpiresHeader(this.App.GetTime().AddYears(1))
            .SetLastModifiedHeader(this.App.GetTime())
            .SetMethod(HttpMethod.Post);
        await using (var response = await request.SendStream())
        {
            response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the request should return status OK");
            response.Content.ReadByte().Should().BePositive("the response stream should not be empty");
        }

        await webApp.StopAsync();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public async Task TestPerformanceWebServer()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        this.AssertPerformance(() => this.TestPerformanceWebServerInternal(this.App));

        await webApp.StopAsync();
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

    private static IApp CreateApp()
    {
        var app = TestApp.Create<WebServerModule, WebClientModule>();
        app.ConfigService.Get<AppIdConfig>().Id = Guid.NewGuid();
        app.Start();
        app.GetEventHub().Subscribe<IConfigureWebAppBuilder>(WebServerTests.ConfigureWebAppBuilder);
        app.GetEventHub().Subscribe<IConfigureWebApp>(WebServerTests.ConfigureWebApp);
        return app;
    }

    private static void ConfigureWebAppBuilder(IConfigureWebAppBuilder args)
    {
        args.Builder.Services.AddControllers();
    }

    private static void ConfigureWebApp(IConfigureWebApp args)
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

    private async Task TestPerformanceWebServerInternal(IApp app)
    {
        for (var i = 0; i < 100; i++)
        {
            await app.GetFactoryService()
                .GetHttpRequest()
                .SetUrl(WebServerTests.ConnectionTestServiceUrl)
                .Send<string>();
        }
    }
    #endregion

    #region Private fields and constants
    private const string ServerBaseAddress = "http://localhost:1337/";
    private const string AppIdServiceUrl = WebServerTests.ServerBaseAddress + "api/appid";
    private const string ConnectionTestServiceUrl = WebServerTests.ServerBaseAddress + "api/testconnection";
    private const string EchoServiceUrl = WebServerTests.ServerBaseAddress + "api/echo";

    private const string Server2BaseAddress = "http://localhost:1338/";
    private const string AppIdService2Url = WebServerTests.Server2BaseAddress + "api/appid";
    #endregion
}
