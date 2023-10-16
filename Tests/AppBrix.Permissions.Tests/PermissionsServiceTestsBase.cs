// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Permissions.Tests;

public abstract class PermissionsServiceTestsBase : TestsBase<PermissionsModule>
{
    #region Tests Roles
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChildNullParent()
    {
        var action = () => this.app.GetPermissionsService().AddChild(null!, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChildEmptyParent()
    {
        var action = () => this.app.GetPermissionsService().AddChild(string.Empty, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChildNullChild()
    {
        var action = () => this.app.GetPermissionsService().AddChild("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChildEmptyChild()
    {
        var action = () => this.app.GetPermissionsService().AddChild("a", string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChildParentEqualsChild()
    {
        var action = () => this.app.GetPermissionsService().AddChild("a", "a");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChildDirectCircularDependency()
    {
        var service = this.app.GetPermissionsService();
        service.AddChild("a", "b");
        var action = () => service.AddChild("b", "a");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChildIndirectCircularDependency()
    {
        var service = this.app.GetPermissionsService();
        service.AddChild("a", "b");
        service.AddChild("b", "c");
        var action = () => service.AddChild("c", "a");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddChild()
    {
        var service = this.app.GetPermissionsService();
        service.AddChild("a", "b");
        service.GetChildren("a").Should().Contain("b", "the child has been added");
        service.GetParents("a").Should().BeEmpty("the parent shouldn't have a child");
        service.GetParents("b").Should().Contain("a", "the parent has been added");
        service.GetChildren("b").Should().BeEmpty("the child shouldn't have a parent");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParentNullChild()
    {
        var action = () => this.app.GetPermissionsService().AddParent(null!, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParentEmptyChild()
    {
        var action = () => this.app.GetPermissionsService().AddParent(string.Empty, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParentNullParent()
    {
        var action = () => this.app.GetPermissionsService().AddParent("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParentEmptyParent()
    {
        var action = () => this.app.GetPermissionsService().AddParent("a", string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParentParentEqualsChild()
    {
        var action = () => this.app.GetPermissionsService().AddParent("a", "a");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParentDirectCircularDependency()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");
        var action = () => service.AddParent("b", "a");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParentIndirectCircularDependency()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");
        service.AddParent("b", "c");
        var action = () => service.AddParent("c", "a");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAddParent()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");
        service.GetParents("a").Should().Contain("b", "the parent has been added");
        service.GetChildren("a").Should().BeEmpty("the child shouldn't have a parent");
        service.GetParents("b").Should().BeEmpty("the parent shouldn't have a child");
        service.GetChildren("b").Should().Contain("a", "the child has been added");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveChildNullParent()
    {
        var action = () => this.app.GetPermissionsService().RemoveChild(null!, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveChildEmptyParent()
    {
        var action = () => this.app.GetPermissionsService().RemoveChild(string.Empty, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveChildNullChild()
    {
        var action = () => this.app.GetPermissionsService().RemoveChild("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveChildEmptyChild()
    {
        var action = () => this.app.GetPermissionsService().RemoveChild("a", string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveChildNonExisting()
    {
        var service = this.app.GetPermissionsService();
        service.RemoveChild("a", "b");
        service.GetParents("a").Should().BeEmpty("no parent has been added");
        service.GetChildren("b").Should().BeEmpty("no child has been added");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveChild()
    {
        var service = this.app.GetPermissionsService();
        service.AddChild("a", "b");
        service.GetChildren("a").Should().Contain("b", "the child has been added");
        service.GetParents("b").Should().Contain("a", "the parent has been added");
        service.RemoveChild("a", "b");
        service.GetChildren("a").Should().BeEmpty("the child has been removed");
        service.GetParents("b").Should().BeEmpty("the parent has been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveParentNullChild()
    {
        var action = () => this.app.GetPermissionsService().RemoveParent(null!, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveParentEmptyChild()
    {
        var action = () => this.app.GetPermissionsService().RemoveParent(string.Empty, "a");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveParentNullParent()
    {
        var action = () => this.app.GetPermissionsService().RemoveParent("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveParentEmptyParent()
    {
        var action = () => this.app.GetPermissionsService().RemoveParent("a", string.Empty);
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
        service.GetParents("a").Should().BeEmpty("the parent has been removed");
        service.GetChildren("b").Should().BeEmpty("the child has been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetParentsNullRole()
    {
        Action action = () => this.app.GetPermissionsService().GetParents(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetParentsEmptyRole()
    {
        Action action = () => this.app.GetPermissionsService().GetParents(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetChildrenNullRole()
    {
        Action action = () => this.app.GetPermissionsService().GetChildren(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetChildrenEmptyRole()
    {
        Action action = () => this.app.GetPermissionsService().GetChildren(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeleteRoleNullRole()
    {
        var action = () => this.app.GetPermissionsService().DeleteRole(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeleteRoleEmptyRole()
    {
        var action = () => this.app.GetPermissionsService().DeleteRole(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeleteRoleUnusedRole()
    {
        this.app.GetPermissionsService().DeleteRole("a");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeleteRole()
    {
        var service = this.app.GetPermissionsService();
        service.AddChild("a", "b");
        service.AddChild("b", "c");
        service.Allow("a", "p-a");
        service.Allow("b", "p-b");

        service.DeleteRole("b");

        service.GetParents("b").Should().BeEmpty("the role should ha been removed with its parents");
        service.GetChildren("b").Should().BeEmpty("the role should ha been removed with its children");
        service.GetAllowed("b").Should().BeEmpty("the role should ha been removed with its allowed permissions");
        service.GetDenied("b").Should().BeEmpty("the role should ha been removed with its denied permissions");
        service.GetChildren("a").Should().BeEmpty("the only child has been removed");
        service.GetParents("c").Should().BeEmpty("the only parent has been removed");
        service.Check("c", "a-b").Should().BeFalse("the parent has been removed with its parents");
        service.Check("c", "p-b").Should().BeFalse("the parent has been removed with its permissions");
    }
    #endregion

    #region Tests Permissions
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAllowNullRole()
    {
        var action = () => this.app.GetPermissionsService().Allow(null!, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAllowEmptyRole()
    {
        var action = () => this.app.GetPermissionsService().Allow(string.Empty, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAllowNullPermission()
    {
        var action = () => this.app.GetPermissionsService().Allow("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAllowEmptyPermission()
    {
        var action = () => this.app.GetPermissionsService().Allow("a", string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDenyNullRole()
    {
        var action = () => this.app.GetPermissionsService().Deny(null!, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDenyEmptyRole()
    {
        var action = () => this.app.GetPermissionsService().Deny(string.Empty, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDenyNullPermission()
    {
        var action = () => this.app.GetPermissionsService().Deny("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDenyEmptyPermission()
    {
        var action = () => this.app.GetPermissionsService().Deny("a", string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsetNullRole()
    {
        var action = () => this.app.GetPermissionsService().Unset(null!, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsetEmptyRole()
    {
        var action = () => this.app.GetPermissionsService().Unset(string.Empty, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsetNullPermission()
    {
        var action = () => this.app.GetPermissionsService().Unset("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsetEmptyPermission()
    {
        var action = () => this.app.GetPermissionsService().Unset("a", string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHasPermissionNullRole()
    {
        Action action = () => this.app.GetPermissionsService().Check(null!, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHasPermissionEmptyRole()
    {
        Action action = () => this.app.GetPermissionsService().Check(string.Empty, "p");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHasPermissionNullPermission()
    {
        Action action = () => this.app.GetPermissionsService().Check("a", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHasPermissionEmptyPermission()
    {
        Action action = () => this.app.GetPermissionsService().Check("a", string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetAllowedNullRole()
    {
        Action action = () => this.app.GetPermissionsService().GetAllowed(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetAllowedEmptyRole()
    {
        Action action = () => this.app.GetPermissionsService().GetAllowed(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDeniedNullRole()
    {
        Action action = () => this.app.GetPermissionsService().GetDenied(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetDeniedEmptyRole()
    {
        Action action = () => this.app.GetPermissionsService().GetDenied(string.Empty);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAllowDenyUnset()
    {
        var service = this.app.GetPermissionsService();
        service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
        service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
        service.Check("a", "p").Should().BeFalse("no permission has been added");

        service.Allow("a", "p");
        service.GetAllowed("a").Should().Equal(new[] { "p" }, because: "permission should have been allowed");
        service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
        service.Check("a", "p").Should().BeTrue("permission should have been allowed");

        service.Deny("a", "p");
        service.GetAllowed("a").Should().BeEmpty("permission has been denied");
        service.GetDenied("a").Should().Equal(new[] { "p" }, because: "permission should have been denied");
        service.Check("a", "p").Should().BeFalse("permission should have been denied");

        service.Unset("a", "p");
        service.GetAllowed("a").Should().BeEmpty("permission has been unset");
        service.GetDenied("a").Should().BeEmpty("permission has been unset");
        service.Check("a", "p").Should().BeFalse("permission has been unset");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDenyAllowUnset()
    {
        var service = this.app.GetPermissionsService();
        service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
        service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
        service.Check("a", "p").Should().BeFalse("no permission has been added");

        service.Deny("a", "p");
        service.GetAllowed("a").Should().BeEmpty("permission has been denied");
        service.GetDenied("a").Should().Equal(new[] { "p" }, because: "permission should have been denied");
        service.Check("a", "p").Should().BeFalse("permission should have been denied");

        service.Allow("a", "p");
        service.GetAllowed("a").Should().Equal(new[] { "p" }, because: "permission should have been allowed");
        service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
        service.Check("a", "p").Should().BeTrue("permission should have been allowed");

        service.Unset("a", "p");
        service.GetAllowed("a").Should().BeEmpty("permission has been unset");
        service.GetDenied("a").Should().BeEmpty("permission has been unset");
        service.Check("a", "p").Should().BeFalse("permission has been unset");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestParentAllowedChildUnset()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");

        service.Allow("b", "p");
        service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
        service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
        service.Check("a", "p").Should().BeTrue("parent allowed permission should have been inherited");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestParentDeniedChildUnset()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");

        service.Deny("b", "p");
        service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
        service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
        service.Check("a", "p").Should().BeFalse("parent denied permission should have been inherited");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestParentAllowedChildDenied()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");

        service.Allow("b", "p");
        service.Deny("a", "p");
        service.Check("a", "p").Should().BeFalse("child permission should override parent");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestParentDeniedChildAllowed()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");

        service.Deny("b", "p");
        service.Allow("a", "p");
        service.Check("a", "p").Should().BeTrue("child permission should override parent");
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
        service.Check("a", "p").Should().BeTrue("grandparent allowed permission should have been inherited");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestOneParentAllowedAnotherUnsetChildUnset()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");
        service.AddParent("a", "c");

        service.Allow("b", "p");
        service.GetAllowed("a").Should().BeEmpty("no permissions have been allowed");
        service.GetDenied("a").Should().BeEmpty("no permissions have been denied");
        service.Check("a", "p").Should().BeTrue("parent allowed permission should have been inherited");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestOneParentAllowedAnotherDeniedChildUnset()
    {
        var service = this.app.GetPermissionsService();
        service.AddParent("a", "b");
        service.AddParent("a", "c");

        service.Allow("b", "p");
        service.Deny("c", "p");
        service.Check("a", "p").Should().BeFalse("parent with denied permission should have priority");
    }
    #endregion
}
