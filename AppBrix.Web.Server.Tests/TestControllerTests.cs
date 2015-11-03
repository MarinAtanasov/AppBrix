// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Tests;
using FluentAssertions;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;
using System.Net;
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
            using (var server = this.CreateTestServer(TestControllerTests.ServerBaseUrl))
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
            using (var server1 = this.CreateTestServer(TestControllerTests.ServerBaseUrl))
            using (var client1 = server1.CreateClient())
            using (var server2 = this.CreateTestServer(TestControllerTests.ServerBaseUrl))
            using (var client2 = server2.CreateClient())
            {
                var response1 = await client1.GetAsync(TestControllerTests.TestConnectionService2Url);
                response1.StatusCode.Should().Be(HttpStatusCode.OK, "the first app's call should reach the second app's service");
                var result1 = bool.Parse(await response1.Content.ReadAsStringAsync());
                result1.Should().BeTrue("this is the expected result when testing the connection from the first to the second app");

                var response2 = await client2.GetAsync(TestControllerTests.TestConnectionServiceUrl);
                response2.StatusCode.Should().Be(HttpStatusCode.OK, "the second app's call should reach the first app's service");
                var result2 = bool.Parse(await response2.Content.ReadAsStringAsync());
                result2.Should().BeTrue("this is the expected result when testing the connection from the first to the second app");
            }
        }
        #endregion

        #region Private methods
        private TestServer CreateTestServer(Uri baseAddress, IApp app = null)
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
            server.BaseAddress = baseAddress;
            return server;
        }
        #endregion

        #region Private fields and constants
        private const string ServerBaseAddress = "http://localhost:1337/";
        private static readonly Uri ServerBaseUrl = new Uri(TestControllerTests.ServerBaseAddress);
        private const string TestConnectionServiceUrl = TestControllerTests.ServerBaseAddress + "api/testconnection";

        private const string Server2BaseAddress = "http://localhost:1338/";
        private static readonly Uri Server2BaseUrl = new Uri(TestControllerTests.Server2BaseAddress);
        private const string TestConnectionService2Url = TestControllerTests.Server2BaseAddress + "api/testconnection";
        #endregion
    }
}
