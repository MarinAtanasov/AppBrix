// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    internal class InitializeContext : IInitializeContext
    {
        #region Construction
        public InitializeContext(IApp app)
        {
            this.App = app;
        }
        #endregion

        #region Properties
        public IApp App { get; private set; }
        #endregion
    }
}
