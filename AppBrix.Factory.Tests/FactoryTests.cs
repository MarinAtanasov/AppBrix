// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Exceptions;
using AppBrix.Factory.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AppBrix.Factory.Tests
{
    [TestClass]
    public class FactoryTests
    {
        #region Tests
        [TestMethod]
        public void TestFactoryDefaultConstructorCall()
        {
            var factory = this.GetFactory();
            var defConstObj = factory.Get<DefaultConstructorClass>();
            Assert.IsNotNull(defConstObj, "Should not return null. Should call default constructor.");
            Assert.IsTrue(defConstObj.ConstructorCalled, "Default constructor not called.");
        }

        [TestMethod]
        [ExpectedException(typeof(DefaultConstructorMissingException))]
        public void TestFactoryNonDefaultConstructorCall()
        {
            var factory = this.GetFactory();
            var defConstObj = factory.Get<NonDefaultConstructorClass>();
        }

        [TestMethod]
        public void TestFactoryRegistered()
        {
            var factory = this.GetFactory();
            factory.Register<FactoryTests>(() => this);
            var returned = factory.Get<FactoryTests>();
            Assert.IsNotNull(returned, "Should not return null.");
            Assert.AreSame(this, returned, "The factory should return the same object.");
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceFactory()
        {
            var factory = this.GetFactory();
            factory.Register<FactoryTests>(() => this);
            for (int i = 0; i < 100000; i++)
            {
                factory.Get<FactoryTests>();
            }
        }
        #endregion

        #region Private methods
        private IFactory GetFactory()
        {
            return new DefaultFactory();
        }
        #endregion
    }
}
