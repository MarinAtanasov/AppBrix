// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using System;
using System.Linq;
using AppBrix.Lifecycle;

namespace AppBrix.Tests.Mocks
{
    internal class SimpleModuleMock : IModule
    {
        public int LoadPriority { get { return 25; } }

        public bool IsInitialized { get; private set; }

        public bool IsUninitialized { get; private set; }

        public void Initialize(IInitializeContext context)
        {
            this.IsInitialized = true;
        }

        public void Uninitialize()
        {
            this.IsUninitialized = true;
        }
    }
}
