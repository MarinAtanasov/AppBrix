// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;

namespace AppBrix.Permissions.Tests;

[TestClass]
public sealed class PermissionsServiceTests : PermissionsServiceTestsBase
{
    #region Setup and cleanup
    protected override void Initialize()
    {
        this.App.ConfigService.GetPermissionsConfig().EnableCaching = false;
        this.App.Start();
    }
    #endregion

    #region Tests
    [Test, Performance]
    public void TestPerformanceHasPermission()
    {
        var service = this.App.GetPermissionsService();
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

        this.AssertPerformance(this.TestPerformanceHasPermissionInternal);
    }

    [Test, Performance]
    public void TestPerformanceAddPermission()
    {
        var service = this.App.GetPermissionsService();
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

        this.AssertPerformance(this.TestPerformanceAddPermissionInternal);
    }
    #endregion

    #region Private methods
    private void TestPerformanceHasPermissionInternal()
    {
        var service = this.App.GetPermissionsService();
        for (var i = 0; i < 25000; i++)
        {
            service.Check("a", "p");
            service.Check("a", "p1");
            service.Check("a", "p22");
        }
    }

    private void TestPerformanceAddPermissionInternal()
    {
        var service = this.App.GetPermissionsService();
        for (var i = 0; i < 75000; i++)
        {
            var item = (i % 20).ToString();
            service.Allow("a", item);
            service.Deny("a1", item);
            service.Allow("a22", item);
        }
    }
    #endregion
}
