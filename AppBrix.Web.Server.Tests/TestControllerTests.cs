// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
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
            using (var server = this.CreateTestServer())
            using (var client = server.CreateClient())
            {
                var response = await client.GetAsync(TestControllerTests.TestConnectionServiceUrl);
                response.StatusCode.Should().Be(HttpStatusCode.OK, "the request should return status OK");
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                result.Should().BeTrue("this is the expected result when testing the connection");
            }
        }
        #endregion

        #region Private methods
        private TestServer CreateTestServer()
        {
            Action<IApplicationBuilder> app = builder =>
            {
                builder.UseMvc();
            };
            Action<IServiceCollection> services = collection =>
            {
                collection.AddMvc();
                collection.AddApp(TestUtils.CreateTestApp());
            };

            var server = TestServer.Create(app, services);
            server.BaseAddress = this.ServerBaseUrl;
            return server;
        }
        #endregion

        #region Private fields and constants
        private const string ServerBaseAddress = "http://localhost:1337/";
        private readonly Uri ServerBaseUrl = new Uri(TestControllerTests.ServerBaseAddress);
        private const string TestConnectionServiceUrl = TestControllerTests.ServerBaseAddress + "api/testconnection";
        #endregion
    }
}
