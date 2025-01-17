// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Web.Client;
using AppBrix.Web.Client.Contracts;
using AppBrix.Web.Server.Events;
using AppBrix.Web.Server.Tests.Mocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AppBrix.Web.Server.Tests;

[TestClass]
public sealed class WebServerTests : TestsBase<WebServerModule, WebClientModule>
{
    #region Test lifecycle
    protected override void Initialize() => WebServerTests.CreateApp(this.App);
    #endregion

    #region Tests
    [Test, Functional]
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
        this.Assert(response.StatusCode == (int)HttpStatusCode.OK, "the GET request should return status OK");

        var postResponse = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.ConnectionTestServiceUrl)
            .SetClientName(string.Empty)
            .SetContent(42)
            .SetHeader("Content-Type", "application/json")
            .SetMethod(HttpMethod.Post)
            .SetVersion(new Version(1, 1))
            .Send<int>();
        this.Assert(postResponse.StatusCode == (int)HttpStatusCode.OK, "the POST request should return status OK");
        this.Assert(postResponse.Content == 42, "the request should return the same integer that has been passed");

        await webApp.StopAsync();
    }

    [Test, Functional]
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
        this.Assert(response1.StatusCode == (int)HttpStatusCode.OK, "the first app's call should reach the second app's service");
        this.Assert(response1.Content.Id == app2.ConfigService.Get<AppIdConfig>().Id, "the first app should receive the second app's id");

        app2.Container.Register(app1Client);
        var response2 = await app2.GetFactoryService().GetHttpRequest()
            .SetUrl(WebServerTests.AppIdServiceUrl)
            .Send<AppIdMessage>();
        this.Assert(response2.StatusCode == (int)HttpStatusCode.OK, "the second app's call should reach the first app's service");
        this.Assert(response2.Content.Id == app1.ConfigService.Get<AppIdConfig>().Id, "the second app should receive the first app's id");

        await webApp2.StopAsync();
        await webApp1.StopAsync();
    }

    [Test, Functional]
    public async Task TestEchoGetString()
    {
        await using var webApp = this.CreateTestWebApp(this.App);
        using var client = webApp.GetTestClient();
        this.App.Container.Register(client);

        var response = await this.App.GetFactoryService().GetHttpRequest()
            .SetUrl($"{WebServerTests.EchoServiceUrl}/{nameof(this.TestEchoGetString)}")
            .Send<string>();
        this.Assert(response.StatusCode == (int)HttpStatusCode.OK, "the request should return status OK");
        this.Assert(response.ReasonPhrase == HttpStatusCode.OK.ToString(), "the request should return status OK");
        this.Assert(response.Version == new Version(1, 1), "the version of the response should be 1.1");
        this.Assert(response.Content == nameof(this.TestEchoGetString), "the response should echo the request");
        this.Assert(response.Headers.Count == 1, "only the content-type header should be returned");
        this.Assert(response.Headers["Content-Type"].Count() == 1, "the content type should be only a utf-8 string");
        this.Assert(response.Headers["Content-Type"].Contains("text/plain; charset=utf-8"), "the content type should be a utf-8 string");

        await webApp.StopAsync();
    }

    [Test, Functional]
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
        this.Assert(response.StatusCode == (int)HttpStatusCode.OK, "the request should return status OK");
        this.Assert(response.Content.Equals(model), "the response should echo the request");

        await webApp.StopAsync();
    }

    [Test, Functional]
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
        this.Assert(response.StatusCode == (int)HttpStatusCode.OK, "the request should return status OK");
        this.Assert(response.Content.Equals(model), "the response should echo the request");

        await webApp.StopAsync();
    }

    [Test, Functional]
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
        this.Assert(response.StatusCode == (int)HttpStatusCode.OK, "the request should return status OK");
        this.Assert(response.Content.Length > 0, "the response should echo the request");

        await webApp.StopAsync();
    }

    [Test, Functional]
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
            this.Assert(response.StatusCode == (int)HttpStatusCode.OK, "the request should return status OK");
            this.Assert(response.Content.ReadByte() > 0, "the response stream should not be empty");
        }

        await webApp.StopAsync();
    }

    [Test, Functional]
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
            this.Assert(response.StatusCode == (int)HttpStatusCode.OK, "the request should return status OK");
            this.Assert(response.Content.ReadByte() > 0, "the response stream should not be empty");
        }

        await webApp.StopAsync();
    }

    [Test, Performance]
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

    private static IApp CreateApp() => WebServerTests.CreateApp(TestApp.Create<WebServerModule, WebClientModule>());

    private static IApp CreateApp(IApp app)
    {
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
