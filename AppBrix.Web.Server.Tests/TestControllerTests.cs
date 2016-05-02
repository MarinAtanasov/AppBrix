// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Factory;
using AppBrix.Resolver;
using AppBrix.Tests;
using AppBrix.Web.Client;
using FluentAssertions;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace AppBrix.Web.Server.Tests
{
    public class TestControllerTests
    {
        #region Setup and cleanup
        public TestControllerTests()
        {
        }
        #endregion

        #region Tests
        [Fact]
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
        
        [Fact]
        public async void TestConnectionBetweenTwoApps()
        {
            var app1 = TestUtils.CreateTestApp(typeof(ResolverModule), typeof(FactoryModule), typeof(WebClientModule), typeof(WebServerModule));
            app1.Start();
            var app2 = TestUtils.CreateTestApp(typeof(ResolverModule), typeof(FactoryModule), typeof(WebClientModule), typeof(WebServerModule));
            app2.Start();
            try
            {
                using (var server1 = this.CreateTestServer(TestControllerTests.ServerBaseAddress, app1))
                using (var server2 = this.CreateTestServer(TestControllerTests.Server2BaseAddress, app2))
                {
                    app1.GetFactory().Register<HttpClient>(() => server2.CreateClient());
                    var response1 = await app1.GetFactory().Get<IHttpCall>().SetUrl(TestControllerTests.AppIdService2Url).MakeCall<string>();
                    response1.StatusCode.Should().Be((int)HttpStatusCode.OK, "the first app's call should reach the second app's service");
                    var result1 = Guid.Parse(response1.Content.Data);
                    result1.Should().Be(app2.Id, "the first app should receive the second app's id");

                    app2.GetFactory().Register<HttpClient>(() => server1.CreateClient());
                    var response2 = await app2.GetFactory().Get<IHttpCall>().SetUrl(TestControllerTests.AppIdServiceUrl).MakeCall<string>();
                    response2.StatusCode.Should().Be((int)HttpStatusCode.OK, "the second app's call should reach the first app's service");
                    var result2 = Guid.Parse(response2.Content.Data);
                    result2.Should().Be(app1.Id, "the second app should receive the first app's id");
                }
            }
            finally
            {
                app2.Stop();
                app1.Stop();
            }
        }
        #endregion

        #region Private methods
        private TestServer CreateTestServer(string baseAddress, IApp app = null)
        {
            Action<IApplicationBuilder> application = builder =>
            {
                builder.UseMvc();
            };
            Action<IServiceCollection> services = collection =>
            {
                collection.AddMvc();
                collection.AddApp(app ?? TestUtils.CreateTestApp());
            };

            var server = TestServer.Create(application, services);
            server.BaseAddress = new Uri(baseAddress);
            return server;
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
