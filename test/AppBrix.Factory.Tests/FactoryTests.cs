// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Factory.Tests.Mocks;
using AppBrix.Tests;
using AppBrix.Utils.Exceptions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Factory.Tests
{
    public class FactoryTests
    {
        #region Setup and cleanup
        public FactoryTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ContainerModule),
                typeof(FactoryModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestFactoryDefaultConstructorCall()
        {
            var factory = this.GetFactory();
            var defConstObj = factory.Get<DefaultConstructorClass>();
            defConstObj.Should().NotBeNull("the factory should call the default constructor");
            defConstObj.ConstructorCalled.Should().BeTrue("the default constructor should be called");
        }

        [Fact]
        public void TestFactoryNonDefaultConstructorCall()
        {
            var factory = this.GetFactory();
            Action action = () => factory.Get<NonDefaultConstructorClass>();
            action.ShouldThrow<DefaultConstructorNotFoundException>();
        }

        [Fact]
        public void TestFactoryRegistered()
        {
            var factory = this.GetFactory();
            factory.Register(() => this);
            var returned = factory.Get<FactoryTests>();
            returned.Should().NotBeNull("the factory should return an object");
            returned.Should().BeSameAs(this, "the factory should return the same object");
        }

        [Fact]
        public void TestPerformanceFactory()
        {
            Action action = this.TestPerformanceFactoryInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Reinitialize();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
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
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
