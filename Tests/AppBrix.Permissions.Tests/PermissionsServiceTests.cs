// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Tests;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Permissions.Tests
{
    public sealed class PermissionsServiceTests : TestsBase
    {
        #region Setup and cleanup
        public PermissionsServiceTests() : base(TestUtils.CreateTestApp<PermissionsModule>()) => this.app.Start();
        #endregion

        #region Tests Parents
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParentNullRole()
        {
            Action action = () => this.app.GetPermissionsService().AddParent(null, "a");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParentEmptyRole()
        {
            Action action = () => this.app.GetPermissionsService().AddParent(string.Empty, "a");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParentNullParent()
        {
            Action action = () => this.app.GetPermissionsService().AddParent("a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParentEmptyParent()
        {
            Action action = () => this.app.GetPermissionsService().AddParent("a", string.Empty);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParentParentEqualsChild()
        {
            Action action = () => this.app.GetPermissionsService().AddParent("a", "a");
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParentDirectCircularDependency()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");
            Action action = () => service.AddParent("b", "a");
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParentIndirectCircularDependency()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");
            service.AddParent("b", "c");
            Action action = () => this.app.GetPermissionsService().AddParent("c", "a");
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAddParent()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");
            service.GetParents("a").Should().Contain("b", "the parent has been added");
            service.GetChildren("b").Should().Contain("a", "the child has been added");
            service.GetParents("b").Should().BeEmpty("the parent shouldn't have a child");
            service.GetChildren("a").Should().BeEmpty("the child shouldn't have a parent");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRemoveParentNullRole()
        {
            Action action = () => this.app.GetPermissionsService().RemoveParent(null, "a");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRemoveParentEmptyRole()
        {
            Action action = () => this.app.GetPermissionsService().RemoveParent(string.Empty, "a");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRemoveParentNullParent()
        {
            Action action = () => this.app.GetPermissionsService().RemoveParent("a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRemoveParentEmptyParent()
        {
            Action action = () => this.app.GetPermissionsService().RemoveParent("a", string.Empty);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRemoveParentNonExisting()
        {
            var service = this.app.GetPermissionsService();
            service.RemoveParent("a", "b");
            service.GetParents("a").Should().BeEmpty("no parent has been added");
            service.GetChildren("b").Should().BeEmpty("no child has been added");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRemoveParent()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");
            service.GetParents("a").Should().Contain("b", "the parent has been added");
            service.GetChildren("b").Should().Contain("a", "the child has been added");
            service.RemoveParent("a", "b");
            service.GetParents("a").Should().BeEmpty("the parent has been added");
            service.GetChildren("b").Should().BeEmpty("the child has been added");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGetParentsNullRole()
        {
            Action action = () => this.app.GetPermissionsService().GetParents(null);
            action.Should().Throw<ArgumentNullException>();
        }
        #endregion

        #region Tests Permissions
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAllowNullRole()
        {
            Action action = () => this.app.GetPermissionsService().Allow(null, "p");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAllowNullPermission()
        {
            Action action = () => this.app.GetPermissionsService().Allow("a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestDenyNullRole()
        {
            Action action = () => this.app.GetPermissionsService().Deny(null, "p");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestDenyNullPermission()
        {
            Action action = () => this.app.GetPermissionsService().Deny("a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUnsetNullRole()
        {
            Action action = () => this.app.GetPermissionsService().Unset(null, "p");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUnsetNullPermission()
        {
            Action action = () => this.app.GetPermissionsService().Unset("a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestHasPermissionNullRole()
        {
            Action action = () => this.app.GetPermissionsService().HasPermission(null, "p");
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestHasPermissionNullPermission()
        {
            Action action = () => this.app.GetPermissionsService().HasPermission("a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGetAllowedNullRole()
        {
            Action action = () => this.app.GetPermissionsService().GetAllowed(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGetDeniedNullRole()
        {
            Action action = () => this.app.GetPermissionsService().GetDenied(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAllowDenyUnset()
        {
            var service = this.app.GetPermissionsService();
            service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeFalse("no permission has been added");

            service.Allow("a", "p");
            service.GetAllowed("a").Should().Equal(new[] { "p" }, because: "permission should have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeTrue("permission should have been allowed");

            service.Deny("a", "p");
            service.GetAllowed("a").Should().BeEmpty("permission has been denied");
            service.GetDenied("a").Should().Equal(new[] { "p" }, because: "permission should have been denied");
            service.HasPermission("a", "p").Should().BeFalse("permission should have been denied");

            service.Unset("a", "p");
            service.GetAllowed("a").Should().BeEmpty("permission has been unset");
            service.GetDenied("a").Should().BeEmpty("permission has been unset");
            service.HasPermission("a", "p").Should().BeFalse("permission has been unset");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestDenyAllowUnset()
        {
            var service = this.app.GetPermissionsService();
            service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeFalse("no permission has been added");

            service.Deny("a", "p");
            service.GetAllowed("a").Should().BeEmpty("permission has been denied");
            service.GetDenied("a").Should().Equal(new[] { "p" }, because: "permission should have been denied");
            service.HasPermission("a", "p").Should().BeFalse("permission should have been denied");

            service.Allow("a", "p");
            service.GetAllowed("a").Should().Equal(new[] { "p" }, because: "permission should have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeTrue("permission should have been allowed");

            service.Unset("a", "p");
            service.GetAllowed("a").Should().BeEmpty("permission has been unset");
            service.GetDenied("a").Should().BeEmpty("permission has been unset");
            service.HasPermission("a", "p").Should().BeFalse("permission has been unset");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestParentAllowedChildUnset()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");

            service.Allow("b", "p");
            service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeTrue("parent allowed permission should have been inherited");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestParentDeniedChildUnset()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");

            service.Deny("b", "p");
            service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeFalse("parent denied permission should have been inherited");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestParentAllowedChildDenied()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");

            service.Allow("b", "p");
            service.Deny("a", "p");
            service.HasPermission("a", "p").Should().BeFalse("child permission should override parent");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestParentDeniedChildAllowed()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");

            service.Deny("b", "p");
            service.Allow("a", "p");
            service.HasPermission("a", "p").Should().BeTrue("child permission should override parent");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGrandparentAllowedChildUnset()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");
            service.AddParent("b", "c");

            service.Allow("c", "p");
            service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeTrue("grandparent allowed permission should have been inherited");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestOneParentAllowedChildUnset()
        {
            var service = this.app.GetPermissionsService();
            service.AddParent("a", "b");
            service.AddParent("a", "c");
            service.AddParent("a", "d");

            service.Deny("b", "p");
            service.Allow("c", "p");
            service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
            service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
            service.HasPermission("a", "p").Should().BeTrue("parent allowed permission should have been inherited");
        }
        #endregion

        #region Test Performance
        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceHasPermission()
        {
            this.app.ConfigService.GetPermissionsConfig().EnableCaching = false;
            this.app.Reinitialize();
            var service = this.app.GetPermissionsService();
            service.Allow("a", "p");
            service.AddParent("a", "a1");
            service.Allow("a1", "p1");
            service.AddParent("a", "a2");
            service.Deny("a2", "p1");
            service.Allow("a2", "p2");
            service.AddParent("a2", "a21");
            service.Allow("a21", "p21");
            service.AddParent("a2", "a22");
            service.Allow("a22", "p22");

            void action() => this.TestPerformanceHasPermissionInternal(15000);
            TestUtils.TestPerformance(action);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceHasPermissionCached()
        {
            var service = this.app.GetPermissionsService();
            service.Allow("a", "p");
            service.AddParent("a", "a1");
            service.Allow("a1", "p1");
            service.AddParent("a", "a2");
            service.Deny("a2", "p1");
            service.Allow("a2", "p2");
            service.AddParent("a2", "a21");
            service.Allow("a21", "p21");
            service.AddParent("a2", "a22");
            service.Allow("a22", "p22");

            void action() => this.TestPerformanceHasPermissionInternal(100000);
            TestUtils.TestPerformance(action);
        }

        private void TestPerformanceHasPermissionInternal(int repeats)
        {
            var service = this.app.GetPermissionsService();
            for (var i = 0; i < repeats; i++)
            {
                service.HasPermission("a", "p");
                service.HasPermission("a", "p1");
                service.HasPermission("a", "p22");
            }
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceAddPermission()
        {
            this.app.ConfigService.GetPermissionsConfig().EnableCaching = false;
            this.app.Reinitialize();
            var service = this.app.GetPermissionsService();
            service.Allow("a", "p");
            service.AddParent("a", "a1");
            service.Allow("a1", "p1");
            service.AddParent("a", "a2");
            service.Deny("a2", "p1");
            service.Allow("a2", "p2");
            service.AddParent("a2", "a21");
            service.Allow("a21", "p21");
            service.AddParent("a2", "a22");
            service.Allow("a22", "p22");

            void action() => this.TestPerformanceAddPermissionInternal(70000);
            TestUtils.TestPerformance(action);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceAddPermissionCached()
        {
            var service = this.app.GetPermissionsService();
            service.Allow("a", "p");
            service.AddParent("a", "a1");
            service.Allow("a1", "p1");
            service.AddParent("a", "a2");
            service.Deny("a2", "p1");
            service.Allow("a2", "p2");
            service.AddParent("a2", "a21");
            service.Allow("a21", "p21");
            service.AddParent("a2", "a22");
            service.Allow("a22", "p22");

            void action() => this.TestPerformanceAddPermissionInternal(2000);
            TestUtils.TestPerformance(action);
        }

        private void TestPerformanceAddPermissionInternal(int repeats)
        {
            var service = this.app.GetPermissionsService();
            for (var i = 0; i < repeats; i++)
            {
                var item = (i % 20).ToString();
                service.Allow("a", item);
                service.Deny("a1", item);
                service.Allow("a22", item);
            }
        }
        #endregion
    }
}
