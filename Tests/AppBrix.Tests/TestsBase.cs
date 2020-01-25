// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Tests
{
    public abstract class TestsBase : IDisposable
    {
        #region Setup and cleanup
        public TestsBase(IApp app) => this.app = app;

        public virtual void Dispose()
        {
            try { this.app.Stop(); } catch (InvalidOperationException) { }
        }
        #endregion

        #region Private fields and constants
        protected readonly IApp app;
        #endregion
    }
}
