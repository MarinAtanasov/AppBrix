// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;

namespace AppBrix.Tests.Mocks;

internal sealed class SimpleInstallableModuleMock : SimpleModuleMock
{
    public override IEnumerable<Type> Dependencies => Array.Empty<Type>();

    public bool IsConfigured { get; private set; }

    public bool IsInstalled { get; private set; }

    public bool IsUninstalled { get; private set; }

    protected override void Configure(IConfigureContext context)
    {
        if (this.App != context.App)
            throw new InvalidOperationException($"this.{nameof(App)} should be the same as {nameof(context)}.{nameof(context.App)}.");

        this.IsConfigured = true;
    }

    protected override void Install(IInstallContext context)
    {
        if (this.App != context.App)
            throw new InvalidOperationException($"this.{nameof(App)} should be the same as {nameof(context)}.{nameof(context.App)}.");

        this.IsInstalled = true;
    }

    protected override void Uninstall(IUninstallContext context)
    {
        if (this.App != context.App)
            throw new InvalidOperationException($"this.{nameof(App)} should be the same as {nameof(context)}.{nameof(context.App)}.");

        this.IsUninstalled = true;
    }
}
