// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using System;
using System.Linq;
using AppBrix.Lifecycle;

namespace AppBrix.Tests.Mocks
{
    internal class SimpleModuleMock : ModuleBase
    {
        public bool IsInitialized { get; private set; }

        public bool IsUninitialized { get; private set; }

        protected override void InitializeModule(IInitializeContext context)
        {
            this.IsInitialized = true;
        }

        protected override void UninitializeModule()
        {
            this.IsUninitialized = true;
        }
    }
}
