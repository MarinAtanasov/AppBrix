// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Tests.Mocks
{
    /// <summary>
    /// Used for creating an <see cref="IInitializeContext"/> object for testing purposes.
    /// </summary>
    public sealed class InitializeContextMock : IInitializeContext
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="InitializeContextMock"/>.
        /// </summary>
        /// <param name="app">The current application.</param>
        public InitializeContextMock(IApp app)
        {
            this.App = app;
        }
        #endregion

        #region IInitializeContext implementation
        public IApp App { get; }
        #endregion
    }
}
