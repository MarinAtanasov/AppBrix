// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Modules;
using System;

namespace AppBrix.Tests;

public abstract class TestsBase : IDisposable
{
    #region Setup and cleanup
    protected TestsBase(IApp app) => this.app = app;

    public virtual void Dispose()
    {
        try { this.app.Stop(); } catch (InvalidOperationException) { }
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Private fields and constants
    protected readonly IApp app;
    #endregion
}

public abstract class TestsBase<T> : TestsBase
    where T : class, IModule
{
    #region Setup and cleanup
    protected TestsBase() : base(TestUtils.CreateTestApp<T>())
    {
    }
    #endregion
}

public abstract class TestsBase<T1, T2> : TestsBase
    where T1 : class, IModule
    where T2 : class, IModule
{
    #region Setup and cleanup
    protected TestsBase() : base(TestUtils.CreateTestApp<T1, T2>())
    {
    }
    #endregion
}
