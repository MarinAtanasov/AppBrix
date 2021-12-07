// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Tests;
using AppBrix.Text.Tests.Mocks;
using FluentAssertions;
using System.Text;
using Xunit;

namespace AppBrix.Text.Tests;

public sealed class EncodingProviderWrapperTests : TestsBase
{
    #region Setup and cleanup
    public EncodingProviderWrapperTests() : base(TestUtils.CreateTestApp<TextModule>()) => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEncodingWrapper()
    {
        var encoding = Encoding.UTF8;
        var provider = new EncodingProviderMock(encoding);
        provider.Encoding.Should().BeSameAs(encoding, "provided encoding should be saved");

        app.Container.Register(provider);

        provider.IsGetEncodingWithNameCalled.Should().BeFalse("encoding with name should not be called yet");
        provider.IsGetEncodingWithCodePageCalled.Should().BeFalse("encoding with code page should not be called yet");

        Encoding.GetEncoding("test").Should().BeSameAs(encoding, "wrapper should have called the registered provider with name");
        provider.IsGetEncodingWithNameCalled.Should().BeTrue("encoding with name should be called");

        Encoding.GetEncoding(12).Should().BeSameAs(encoding, "wrapper should have called the registered provider with code page");
        provider.IsGetEncodingWithCodePageCalled.Should().BeTrue("encoding with cade page should be called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEncodingProviderWrapper() => TestUtils.TestPerformance(this.TestPerformanceEncodingProviderWrapperInternal);
    #endregion

    #region Private methods
    private void TestPerformanceEncodingProviderWrapperInternal()
    {
        this.app.Container.Register(new EncodingProviderMock(Encoding.UTF8));

        for (var i = 0; i < 100000; i++)
        {
            Encoding.GetEncoding("str");
            Encoding.GetEncoding(32167);
        }
    }
    #endregion
}
