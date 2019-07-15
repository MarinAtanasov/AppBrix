// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Tests;
using AppBrix.Web.Client;
using AppBrix.Web.Server.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore;
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
            var app = this.CreateWebApp();
            using (var server = this.CreateTestServer(TestControllerTests.ServerBaseAddress, app))
            using (var client = server.CreateClient())
            {
                app.Container.Register(client);

                var response = await app.GetFactoryService().Get<IHttpRequest>()
                    .SetUrl(TestControllerTests.TestConnectionServiceUrl)
                    .SetHeader("x-test", "test")
                    .SetHeader("x-test")
                    .Send()
                    .ConfigureAwait(false);
                response.StatusCode.Should().Be((int)HttpStatusCode.OK, "the GET request should return status OK");

                var postResponse = await app.GetFactoryService().Get<IHttpRequest>()
                    .SetUrl(TestControllerTests.TestConnectionServiceUrl)
                    .SetClientName(string.Empty)
                    .SetContent(42)
                    .SetHeader("Content-Type", "application/json")
                    .SetMethod(HttpMethod.Post)
                    .SetVersion(new Version(1, 1))
                    .Send<string>()
                    .ConfigureAwait(false);
                postResponse.StatusCode.Should().Be((int)HttpStatusCode.OK, "the POST request should return status OK");
                int.Parse(postResponse.Content).Should().Be(42, "the request should return the same integer that has been passed");
            }
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public async void TestConnectionBetweenTwoApps()
        {
            var app1 = this.CreateWebApp();
            var app2 = this.CreateWebApp();
            using (var server1 = this.CreateTestServer(TestControllerTests.ServerBaseAddress, app1))
            using (var app1Client = server1.CreateClient())
            using (var server2 = this.CreateTestServer(TestControllerTests.Server2BaseAddress, app2))
            using (var app2Client = server2.CreateClient())
            {
                app1.Container.Register(app2Client);
                var response1 = await app1.GetFactoryService().Get<IHttpRequest>().SetUrl(TestControllerTests.AppIdService2Url).Send<string>().ConfigureAwait(false);
                response1.StatusCode.Should().Be((int)HttpStatusCode.OK, "the first app's call should reach the second app's service");
                var result1 = Guid.Parse(response1.Content);
                result1.Should().Be(app2.GetConfig<AppIdConfig>().Id, "the first app should receive the second app's id");

                app2.Container.Register(app1Client);
                var response2 = await app2.GetFactoryService().Get<IHttpRequest>().SetUrl(TestControllerTests.AppIdServiceUrl).Send<string>().ConfigureAwait(false);
                response2.StatusCode.Should().Be((int)HttpStatusCode.OK, "the second app's call should reach the first app's service");
                var result2 = Guid.Parse(response2.Content);
                result2.Should().Be(app1.GetConfig<AppIdConfig>().Id, "the second app should receive the first app's id");
            }
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceWebServer()
        {
            var app = this.CreateWebApp();
            using (var server = this.CreateTestServer(TestControllerTests.ServerBaseAddress, app))
            using (var client = server.CreateClient())
            {
                app.Container.Register(client);
                TestUtils.TestPerformance(() => this.TestPerformanceWebServerInternal(app));
            }
        }
        #endregion

        #region Private methods
        private TestServer CreateTestServer(string baseAddress, IApp app)
        {
            return new TestServer(WebHost.CreateDefaultBuilder().UseUrls(baseAddress).UseApp(app))
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        private IApp CreateWebApp()
        {
            var app = TestUtils.CreateTestApp(typeof(WebServerModule), typeof(WebClientModule));
            app.GetConfig<AppIdConfig>().Id = Guid.NewGuid();
            app.Start();
            app.GetEventHub().Subscribe<IConfigureWebHost>(webHost => webHost.Builder.ConfigureServices(services => services.AddMvc()));
            app.GetEventHub().Subscribe<IConfigureApplication>(application => application.Builder.UseMvc());
            return app;
        }

        private void TestPerformanceWebServerInternal(IApp app)
        {
            for (int i = 0; i < 100; i++)
            {
                app.GetFactoryService()
                    .Get<IHttpRequest>()
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
}
