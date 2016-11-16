// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Linq;
using System.Text;
using AppBrix.Text.Tests.Mocks;
using Xunit;

namespace AppBrix.Text.Tests
{
    public class EncodingProviderWrapper
    {
        #region Setup and cleanup
        public EncodingProviderWrapper()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ContainerModule),
                typeof(TextModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestEncodingWrapper()
        {
            var encoding = Encoding.UTF7;
            var provider = new EncodingProviderMock(encoding);
            provider.Encoding.Should().BeSameAs(encoding, "provided encoding should be saved");

            app.GetContainer().Register(provider);

            provider.IsGetEncodingWithNameCalled.Should().BeFalse("encoding with name should not be called yet");
            provider.IsGetEncodingWithCodePageCalled.Should().BeFalse("encoding with code page should not be called yet");

            Encoding.GetEncoding("test").Should().BeSameAs(encoding, "wrapper should have called the registered provider with name");
            provider.IsGetEncodingWithNameCalled.Should().BeTrue("encoding with name should be called");

            Encoding.GetEncoding(12).Should().BeSameAs(encoding, "wrapper should have called the registered provider with code page");
            provider.IsGetEncodingWithCodePageCalled.Should().BeTrue("encoding with cade page should be called");
        }
        #endregion
        
        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
