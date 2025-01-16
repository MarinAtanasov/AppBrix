// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using AppBrix.Text.Tests.Mocks;
using System.Text;
using Xunit;

namespace AppBrix.Text.Tests;

public sealed class EncodingProviderWrapperTests : TestsBase<TextModule>
{
    #region Setup and cleanup
    public EncodingProviderWrapperTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEncodingProvider()
    {
        var encoding = Encoding.UTF8;
        this.Assert(object.ReferenceEquals(Encoding.GetEncoding(encoding.BodyName), encoding), "provider should return encoding by body name");
        this.Assert(object.ReferenceEquals(Encoding.GetEncoding(encoding.CodePage), encoding), "provider should return encoding by code page");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEncodingWrapper()
    {
        var encoding = Encoding.UTF8;
        var provider = new EncodingProviderMock(encoding);
        this.Assert(object.ReferenceEquals(provider.Encoding, encoding), "provided encoding should be saved");

        this.App.Container.Register(provider);

        this.Assert(provider.IsGetEncodingWithNameCalled == false, "encoding with name should not be called yet");
        this.Assert(provider.IsGetEncodingWithCodePageCalled == false, "encoding with code page should not be called yet");

        this.Assert(object.Equals(Encoding.GetEncoding("test"), encoding), "wrapper should have called the registered provider with name");
        this.Assert(provider.IsGetEncodingWithNameCalled, "encoding with name should be called");

        this.Assert(object.ReferenceEquals(Encoding.GetEncoding(12), encoding), "wrapper should have called the registered provider with code page");
        this.Assert(provider.IsGetEncodingWithCodePageCalled, "encoding with cade page should be called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEncodingProviderWrapper() => this.AssertPerformance(this.TestPerformanceEncodingProviderWrapperInternal);
    #endregion

    #region Private methods
    private void TestPerformanceEncodingProviderWrapperInternal()
    {
        this.App.Container.Register(new EncodingProviderMock(Encoding.UTF8));

        for (var i = 0; i < 100000; i++)
        {
            Encoding.GetEncoding("str");
            Encoding.GetEncoding(32167);
        }
    }
    #endregion
}
