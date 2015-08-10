// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Resolver.Tests.Mocks;
using AppBrix.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AppBrix.Resolver.Tests
{
    [TestClass]
    public class ResolverTests
    {
        #region Setup and cleanup
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ResolverTests.app = TestUtils.CreateTestApp(
                typeof(ResolverModule));
            ResolverTests.app.Start();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ResolverTests.app.Stop();
            ResolverTests.app = null;
        }

        [TestCleanup]
        public void Cleanup()
        {
            ResolverTests.app.Reinitialize();
        }
        #endregion

        #region Tests
        [TestMethod]
        public void TestGetResolver()
        {
            var resolver = this.GetResolver();
            Assert.IsNotNull(resolver, "Unable to get the resolver.");
            Assert.IsInstanceOfType(resolver, typeof(DefaultResolver), "Returned resolver is not of default type.");
            var resolver2 = this.GetResolver();
            Assert.IsNotNull(resolver2, "Second call did not return a resolver.");
            Assert.IsInstanceOfType(resolver2, typeof(DefaultResolver), "Second returned resolver is not of default type.");
            Assert.AreSame(resolver, resolver2, "Returned a different instance of the resolver.");
        }

        [TestMethod]
        public void TestResolveByInterface()
        {
            var resolver = (DefaultResolver)this.GetResolver();
            var iResolver = resolver.Get<IResolver>();
            Assert.IsNotNull(iResolver, "Unable to resolve the IResolver interface.");
            Assert.AreSame(resolver, iResolver, "Returned IResolver is a different instance.");
        }

        [TestMethod]
        public void TestResolveByClass()
        {
            var resolver = (DefaultResolver)this.GetResolver();
            var resolved = resolver.Get<DefaultResolver>();
            Assert.IsNotNull(resolved, "Unable to resolve the Resolver class.");
            Assert.AreSame(resolver, resolved, "Returned Resolver is a different instance.");
        }

        [TestMethod]
        public void TestResolveByBaseClass()
        {
            var resolver = this.GetResolver();
            var original = new ResolverTestsChild();
            resolver.Register(original);
            var resolved = resolver.Get<ResolverTests>();
            Assert.IsNotNull(resolved, "Unable to resolve the Parent class.");
            Assert.AreSame(original, resolved, "Returned Child is a different instance.");
        }

        [TestMethod]
        public void TestResolveAllNoElements()
        {
            var resolver = this.GetResolver();
            var resolved = resolver.GetAll().OfType<ResolverTests>();
            Assert.IsNotNull(resolved, "Resolved collection should not be null.");
            Assert.AreEqual(0, resolved.Count(), "Resolved collection should be empty.");
        }

        [TestMethod]
        public void TestResolveAllOneElement()
        {
            var resolver = (DefaultResolver)this.GetResolver();
            var resolved = resolver.GetAll().OfType<IResolver>();
            Assert.IsNotNull(resolved, "Resolved collection should not be null.");
            Assert.AreEqual(1, resolved.Count(), "Resolved collection should have 1 element.");
            Assert.AreEqual(resolver, resolved.Single(), "Resolved element should be the original resolver.");
        }

        [TestMethod]
        public void TestResolveAllTwoElements()
        {
            var resolver = (DefaultResolver)this.GetResolver();
            resolver.Register(this);
            var second = new ResolverTestsChild();
            resolver.Register(second, second.GetType());
            var resolved = resolver.GetAll().OfType<ResolverTests>();
            Assert.IsNotNull(resolved, "Resolved collection should not be null.");
            Assert.AreEqual(2, resolved.Count(), "Resolved collection should have 2 elements.");
            Assert.AreEqual(this, resolved.First(), "Resolved element 1 should be the original item.");
            Assert.AreEqual(second, resolved.Last(), "Resolved element 2 should be the new item.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRegisterNull()
        {
            var resolver = this.GetResolver();
            resolver.Register<IResolver>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRegisterNullObjectExcplicitType()
        {
            var resolver = this.GetResolver();
            resolver.Register(null, typeof(IResolver));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRegisterNullTypeExcplicitType()
        {
            var resolver = this.GetResolver();
            resolver.Register(resolver, null);
        }

        [TestMethod]
        public void TestObjectBaseTypeNotRegistered()
        {
            var resolver = this.GetResolver();
            resolver.Register(new ResolverTestsChild());
            Assert.IsNull(resolver.Get<object>());
        }

        [TestMethod]
        public void TestDoubleRegistration()
        {
            var resolver = this.GetResolver();
            var resolved = new ResolverTestsChild();
            var resolved2 = new ResolverTestsChild();
            resolver.Register(resolved);
            resolver.Register(resolved2);
            Assert.AreSame(resolved2, resolver.Get<ResolverTestsChild>(), "Object not replaced with second.");
            resolver.Register(resolved);
            Assert.AreSame(resolved, resolver.Get<ResolverTestsChild>(), "Object not replaced with original.");
            Assert.AreEqual(2, resolver.GetAll().OfType<ResolverTestsChild>().Count(), "First object has been removed from history.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegisterGenericObjectError()
        {
            var resolver = this.GetResolver();
            resolver.Register<object>(resolver);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRegisterExplicitObjectTypeError()
        {
            var resolver = this.GetResolver();
            resolver.Register<object>(resolver);
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceResolver()
        {
            var resolver = (DefaultResolver)this.GetResolver();
            for (int i = 0; i < 10000; i++)
            {
                resolver.Register(new ResolverTestsChild());
            }
            for (int i = 0; i < 10000; i++)
            {
                resolver.Get<ResolverTestsChild>();
                resolver.Get<DefaultResolver>();
                resolver.Get<IResolver>();
                resolver.GetAll();
            }
        }
        #endregion

        #region Private methods
        private IResolver GetResolver()
        {
            return ResolverTests.app.GetResolver();
        }
        #endregion

        #region Private fields and constants
        private static IApp app;
        #endregion
    }
}
