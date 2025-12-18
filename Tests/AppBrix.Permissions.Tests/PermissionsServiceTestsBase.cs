// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using System;
using System.Linq;

namespace AppBrix.Permissions.Tests;

public abstract class PermissionsServiceTestsBase : TestsBase<PermissionsModule>
{
	#region Tests Roles
	[Test, Functional]
	public void TestAddChildNullParent()
	{
		var action = () => this.App.GetPermissionsService().AddChild(null!, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddChildEmptyParent()
	{
		var action = () => this.App.GetPermissionsService().AddChild(string.Empty, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddChildNullChild()
	{
		var action = () => this.App.GetPermissionsService().AddChild("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddChildEmptyChild()
	{
		var action = () => this.App.GetPermissionsService().AddChild("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddChildParentEqualsChild()
	{
		var action = () => this.App.GetPermissionsService().AddChild("a", "a");
		this.AssertThrows<InvalidOperationException>(action);
	}

	[Test, Functional]
	public void TestAddChildDirectCircularDependency()
	{
		var service = this.App.GetPermissionsService();
		service.AddChild("a", "b");
		var action = () => service.AddChild("b", "a");
		this.AssertThrows<InvalidOperationException>(action);
	}

	[Test, Functional]
	public void TestAddChildIndirectCircularDependency()
	{
		var service = this.App.GetPermissionsService();
		service.AddChild("a", "b");
		service.AddChild("b", "c");
		var action = () => service.AddChild("c", "a");
		this.AssertThrows<InvalidOperationException>(action);
	}

	[Test, Functional]
	public void TestAddChild()
	{
		var service = this.App.GetPermissionsService();
		service.AddChild("a", "b");
		this.Assert(service.GetChildren("a").Contains("b"), "the child has been added");
		this.Assert(service.GetParents("a").Count == 0, "the parent shouldn't have a child");
		this.Assert(service.GetParents("b").Contains("a"), "the parent has been added");
		this.Assert(service.GetChildren("b").Count == 0, "the child shouldn't have a parent");
	}

	[Test, Functional]
	public void TestAddParentNullChild()
	{
		var action = () => this.App.GetPermissionsService().AddParent(null!, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddParentEmptyChild()
	{
		var action = () => this.App.GetPermissionsService().AddParent(string.Empty, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddParentNullParent()
	{
		var action = () => this.App.GetPermissionsService().AddParent("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddParentEmptyParent()
	{
		var action = () => this.App.GetPermissionsService().AddParent("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAddParentParentEqualsChild()
	{
		var action = () => this.App.GetPermissionsService().AddParent("a", "a");
		this.AssertThrows<InvalidOperationException>(action);
	}

	[Test, Functional]
	public void TestAddParentDirectCircularDependency()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");
		var action = () => service.AddParent("b", "a");
		this.AssertThrows<InvalidOperationException>(action);
	}

	[Test, Functional]
	public void TestAddParentIndirectCircularDependency()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");
		service.AddParent("b", "c");
		var action = () => service.AddParent("c", "a");
		this.AssertThrows<InvalidOperationException>(action);
	}

	[Test, Functional]
	public void TestAddParent()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");
		this.Assert(service.GetParents("a").Contains("b"), "the parent has been added");
		this.Assert(service.GetChildren("a").Count == 0, "the child shouldn't have a parent");
		this.Assert(service.GetParents("b").Count == 0, "the parent shouldn't have a child");
		this.Assert(service.GetChildren("b").Contains("a"), "the child has been added");
	}

	[Test, Functional]
	public void TestRemoveChildNullParent()
	{
		var action = () => this.App.GetPermissionsService().RemoveChild(null!, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveChildEmptyParent()
	{
		var action = () => this.App.GetPermissionsService().RemoveChild(string.Empty, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveChildNullChild()
	{
		var action = () => this.App.GetPermissionsService().RemoveChild("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveChildEmptyChild()
	{
		var action = () => this.App.GetPermissionsService().RemoveChild("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveChildNonExisting()
	{
		var service = this.App.GetPermissionsService();
		service.RemoveChild("a", "b");
		this.Assert(service.GetParents("a").Count == 0, "no parent has been added");
		this.Assert(service.GetChildren("b").Count == 0, "no child has been added");
	}

	[Test, Functional]
	public void TestRemoveChild()
	{
		var service = this.App.GetPermissionsService();
		service.AddChild("a", "b");
		this.Assert(service.GetChildren("a").Contains("b"), "the child has been added");
		this.Assert(service.GetParents("b").Contains("a"), "the parent has been added");

		service.RemoveChild("a", "b");
		this.Assert(service.GetChildren("a").Count == 0, "the child has been removed");
		this.Assert(service.GetParents("b").Count == 0, "the parent has been removed");
	}

	[Test, Functional]
	public void TestRemoveParentNullChild()
	{
		var action = () => this.App.GetPermissionsService().RemoveParent(null!, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveParentEmptyChild()
	{
		var action = () => this.App.GetPermissionsService().RemoveParent(string.Empty, "a");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveParentNullParent()
	{
		var action = () => this.App.GetPermissionsService().RemoveParent("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveParentEmptyParent()
	{
		var action = () => this.App.GetPermissionsService().RemoveParent("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestRemoveParentNonExisting()
	{
		var service = this.App.GetPermissionsService();
		service.RemoveParent("a", "b");
		this.Assert(service.GetParents("a").Count == 0, "no parent has been added");
		this.Assert(service.GetChildren("b").Count == 0, "no child has been added");
	}

	[Test, Functional]
	public void TestRemoveParent()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");
		this.Assert(service.GetParents("a").Contains("b"), "the parent has been added");
		this.Assert(service.GetChildren("b").Contains("a"), "the child has been added");

		service.RemoveParent("a", "b");
		this.Assert(service.GetParents("a").Count == 0, "the parent has been removed");
		this.Assert(service.GetChildren("b").Count == 0, "the child has been removed");
	}

	[Test, Functional]
	public void TestGetParentsNullRole()
	{
		Action action = () => this.App.GetPermissionsService().GetParents(null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestGetParentsEmptyRole()
	{
		Action action = () => this.App.GetPermissionsService().GetParents(string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestGetChildrenNullRole()
	{
		Action action = () => this.App.GetPermissionsService().GetChildren(null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestGetChildrenEmptyRole()
	{
		Action action = () => this.App.GetPermissionsService().GetChildren(string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestDeleteRoleNullRole()
	{
		var action = () => this.App.GetPermissionsService().DeleteRole(null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestDeleteRoleEmptyRole()
	{
		var action = () => this.App.GetPermissionsService().DeleteRole(string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestDeleteRoleUnusedRole()
	{
		this.App.GetPermissionsService().DeleteRole("a");
	}

	[Test, Functional]
	public void TestDeleteRole()
	{
		var service = this.App.GetPermissionsService();
		service.AddChild("a", "b");
		service.AddChild("b", "c");
		service.Allow("a", "p-a");
		service.Allow("b", "p-b");

		service.DeleteRole("b");

		this.Assert(service.GetParents("b").Count == 0, "the role should ha been removed with its parents");
		this.Assert(service.GetChildren("b").Count == 0, "the role should ha been removed with its children");
		this.Assert(service.GetAllowed("b").Count == 0, "the role should ha been removed with its allowed permissions");
		this.Assert(service.GetDenied("b").Count == 0, "the role should ha been removed with its denied permissions");
		this.Assert(service.GetChildren("a").Count == 0, "the only child has been removed");
		this.Assert(service.GetParents("c").Count == 0, "the only parent has been removed");
		this.Assert(service.Check("c", "a-b") == false, "the parent has been removed with its parents");
		this.Assert(service.Check("c", "p-b") == false, "the parent has been removed with its permissions");
	}
	#endregion

	#region Tests Permissions
	[Test, Functional]
	public void TestAllowNullRole()
	{
		var action = () => this.App.GetPermissionsService().Allow(null!, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAllowEmptyRole()
	{
		var action = () => this.App.GetPermissionsService().Allow(string.Empty, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAllowNullPermission()
	{
		var action = () => this.App.GetPermissionsService().Allow("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAllowEmptyPermission()
	{
		var action = () => this.App.GetPermissionsService().Allow("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestDenyNullRole()
	{
		var action = () => this.App.GetPermissionsService().Deny(null!, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestDenyEmptyRole()
	{
		var action = () => this.App.GetPermissionsService().Deny(string.Empty, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestDenyNullPermission()
	{
		var action = () => this.App.GetPermissionsService().Deny("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestDenyEmptyPermission()
	{
		var action = () => this.App.GetPermissionsService().Deny("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestUnsetNullRole()
	{
		var action = () => this.App.GetPermissionsService().Unset(null!, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestUnsetEmptyRole()
	{
		var action = () => this.App.GetPermissionsService().Unset(string.Empty, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestUnsetNullPermission()
	{
		var action = () => this.App.GetPermissionsService().Unset("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestUnsetEmptyPermission()
	{
		var action = () => this.App.GetPermissionsService().Unset("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestHasPermissionNullRole()
	{
		Action action = () => this.App.GetPermissionsService().Check(null!, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestHasPermissionEmptyRole()
	{
		Action action = () => this.App.GetPermissionsService().Check(string.Empty, "p");
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestHasPermissionNullPermission()
	{
		Action action = () => this.App.GetPermissionsService().Check("a", null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestHasPermissionEmptyPermission()
	{
		Action action = () => this.App.GetPermissionsService().Check("a", string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestGetAllowedNullRole()
	{
		Action action = () => this.App.GetPermissionsService().GetAllowed(null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestGetAllowedEmptyRole()
	{
		Action action = () => this.App.GetPermissionsService().GetAllowed(string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestGetDeniedNullRole()
	{
		Action action = () => this.App.GetPermissionsService().GetDenied(null!);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestGetDeniedEmptyRole()
	{
		Action action = () => this.App.GetPermissionsService().GetDenied(string.Empty);
		this.AssertThrows<ArgumentNullException>(action);
	}

	[Test, Functional]
	public void TestAllowDenyUnset()
	{
		var service = this.App.GetPermissionsService();
		this.Assert(service.GetAllowed("a").Count == 0, "no permissions have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p") == false, "no permission has been added");

		service.Allow("a", "p");
		this.Assert(service.GetAllowed("a").Count == 1, "single permission should have been allowed");
		this.Assert(service.GetAllowed("a").Contains("p"), "permission should have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p"), "permission should have been allowed");

		service.Deny("a", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "permission has been denied");
		this.Assert(service.GetDenied("a").Count == 1, "single permission should have been denied");
		this.Assert(service.GetDenied("a").Contains("p"), "permission should have been denied");
		this.Assert(service.Check("a", "p") == false, "permission should have been denied");

		service.Unset("a", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "permission has been unset");
		this.Assert(service.GetDenied("a").Count == 0, "permission has been unset");
		this.Assert(service.Check("a", "p") == false, "permission has been unset");
	}

	[Test, Functional]
	public void TestDenyAllowUnset()
	{
		var service = this.App.GetPermissionsService();
		this.Assert(service.GetAllowed("a").Count == 0, "no permissions have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p") == false, "no permission has been added");

		service.Deny("a", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "permission has been denied");
		this.Assert(service.GetDenied("a").Count == 1, "single permission should have been denied");
		this.Assert(service.GetDenied("a").Contains("p"), "permission should have been denied");
		this.Assert(service.Check("a", "p") == false, "permission should have been denied");

		service.Allow("a", "p");
		this.Assert(service.GetAllowed("a").Count == 1, "permission should have been allowed");
		this.Assert(service.GetAllowed("a").Contains("p"), "permission should have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p"), "permission should have been allowed");

		service.Unset("a", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "permission has been unset");
		this.Assert(service.GetDenied("a").Count == 0, "permission has been unset");
		this.Assert(service.Check("a", "p") == false, "permission has been unset");
	}

	[Test, Functional]
	public void TestParentAllowedChildUnset()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");

		service.Allow("b", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "no permissions have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p"), "parent allowed permission should have been inherited");
	}

	[Test, Functional]
	public void TestParentDeniedChildUnset()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");

		service.Deny("b", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "no permissions have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p") == false, "parent denied permission should have been inherited");
	}

	[Test, Functional]
	public void TestParentAllowedChildDenied()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");

		service.Allow("b", "p");
		service.Deny("a", "p");
		this.Assert(service.Check("a", "p") == false, "child permission should override parent");
	}

	[Test, Functional]
	public void TestParentDeniedChildAllowed()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");

		service.Deny("b", "p");
		service.Allow("a", "p");
		this.Assert(service.Check("a", "p"), "child permission should override parent");
	}

	[Test, Functional]
	public void TestGrandparentAllowedChildUnset()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");
		service.AddParent("b", "c");

		service.Allow("c", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "no permissions have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p"), "grandparent allowed permission should have been inherited");
	}

	[Test, Functional]
	public void TestOneParentAllowedAnotherUnsetChildUnset()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");
		service.AddParent("a", "c");

		service.Allow("b", "p");
		this.Assert(service.GetAllowed("a").Count == 0, "no permissions have been allowed");
		this.Assert(service.GetDenied("a").Count == 0, "no permissions have been denied");
		this.Assert(service.Check("a", "p"), "parent allowed permission should have been inherited");
	}

	[Test, Functional]
	public void TestOneParentAllowedAnotherDeniedChildUnset()
	{
		var service = this.App.GetPermissionsService();
		service.AddParent("a", "b");
		service.AddParent("a", "c");

		service.Allow("b", "p");
		service.Deny("c", "p");
		this.Assert(service.Check("a", "p") == false, "parent with denied permission should have priority");
	}
	#endregion
}
