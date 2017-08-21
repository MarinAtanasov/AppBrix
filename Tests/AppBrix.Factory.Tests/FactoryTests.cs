// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Factory.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Factory.Tests
{
    public sealed class FactoryTests
    {
        #region Setup and cleanup
        public FactoryTests()
        {
            this.app = TestUtils.CreateTestApp(typeof(FactoryModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestFactoryDefaultConstructorCall()
        {
            var factory = this.GetFactory();
            var defConstObj = factory.Get<DefaultConstructorClass>();
            defConstObj.Should().NotBeNull("the factory should call the default constructor");
            defConstObj.ConstructorCalled.Should().BeTrue("the default constructor should be called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestFactoryNonDefaultConstructorCall()
        {
            var factory = this.GetFactory();
            Action action = () => factory.Get<NonDefaultConstructorClass>();
            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestFactoryRegistered()
        {
            var factory = this.GetFactory();
            factory.Register(() => this);
            var returned = factory.Get<FactoryTests>();
            returned.Should().NotBeNull("the factory should return an object");
            returned.Should().BeSameAs(this, "the factory should return the same object");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceFactory()
        {
            TestUtils.TestPerformance(this.TestPerformanceFactoryInternal);
        }
        #endregion

        #region Private methods
        private IFactory GetFactory()
        {
            return this.app.GetFactory();
        }

        private void TestPerformanceFactoryInternal()
        {
            var factory = this.GetFactory();
            for (int i = 0; i < 5000; i++)
            {
                factory.Register(() => this);
            }
            for (int i = 0; i < 500000; i++)
            {
                factory.Get<FactoryTests>();
            }
            this.app.Reinitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}