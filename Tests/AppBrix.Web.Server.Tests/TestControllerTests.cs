// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Factory;
using AppBrix.Tests;
using AppBrix.Web.Client;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using Xunit;

namespace AppBrix.Web.Server.Tests
{
    public sealed class TestControllerTests
    {
        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public async void TestConnection()
        {
            using (var server = this.CreateTestServer(TestControllerTests.ServerBaseAddress))
            using (var client = server.CreateClient())
            {
                var response = await client.GetAsync(TestControllerTests.TestConnectionServiceUrl);
                response.StatusCode.Should().Be(HttpStatusCode.OK, "the request should return status OK");
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                result.Should().BeTrue("this is the expected result when testing the connection");
            }
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public async void TestConnectionBetweenTwoApps()
        {
            var app1 = this.CreateWebApp();
            var app2 = this.CreateWebApp();
            try
            {
                using (var server1 = this.CreateTestServer(TestControllerTests.ServerBaseAddress, app1))
                using (var server2 = this.CreateTestServer(TestControllerTests.Server2BaseAddress, app2))
                {
                    app1.GetFactory().Register(server2.CreateClient);
                    var response1 = await app1.GetFactory().Get<IHttpRequest>().SetUrl(TestControllerTests.AppIdService2Url).Send<string>();
                    response1.StatusCode.Should().Be((int)HttpStatusCode.OK, "the first app's call should reach the second app's service");
                    var result1 = Guid.Parse(response1.Content);
                    result1.Should().Be(app2.Id, "the first app should receive the second app's id");

                    app2.GetFactory().Register(server1.CreateClient);
                    var response2 = await app2.GetFactory().Get<IHttpRequest>().SetUrl(TestControllerTests.AppIdServiceUrl).Send<string>();
                    response2.StatusCode.Should().Be((int)HttpStatusCode.OK, "the second app's call should reach the first app's service");
                    var result2 = Guid.Parse(response2.Content);
                    result2.Should().Be(app1.Id, "the second app should receive the first app's id");
                }
            }
            finally
            {
                app2.Stop();
                app1.Stop();
            }
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceWebServer()
        {
            Action action = this.TestPerformanceWebServerInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion

        #region Private methods
        private TestServer CreateTestServer(string baseAddress, IApp app = null)
        {
            if (app == null)
            {
                app = TestUtils.CreateTestApp();
                app.Start();
            }

            Action<IApplicationBuilder> application = builder =>
            {
                builder.UseMvc();
            };
            Action<IServiceCollection> services = collection =>
            {
                collection.AddApp(app ?? TestUtils.CreateTestApp());
                collection.AddMvc();
            };
            
            var server = new TestServer(new WebHostBuilder().Configure(application).ConfigureServices(services));
            server.BaseAddress = new Uri(baseAddress);
            return server;
        }

        private IApp CreateWebApp()
        {
            var app = TestUtils.CreateTestApp(typeof(ContainerModule), typeof(FactoryModule), typeof(WebClientModule), typeof(WebServerModule));
            app.Start();
            return app;
        }

        private void TestPerformanceWebServerInternal()
        {
            var app = this.CreateWebApp();
            try
            {
                using (var server = this.CreateTestServer(TestControllerTests.ServerBaseAddress, app))
                {
                    app.GetFactory().Register(server.CreateClient);
                    for (int i = 0; i < 50; i++)
                    {
                        var result = app.GetFactory()
                            .Get<IHttpRequest>()
                            .SetUrl(TestControllerTests.TestConnectionServiceUrl)
                            .Send<string>()
                            .Result;
                    }
                }
            }
            finally
            {
                app.Stop();
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
}
