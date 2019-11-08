// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix.Tests.Mocks
{
    internal class SimpleModuleMock : ModuleBase
    {
        public override IEnumerable<Type> Dependencies => Array.Empty<Type>();

        public bool IsInitialized { get; private set; }

        public bool IsUninitialized { get; private set; }

        protected override void Initialize(IInitializeContext context)
        {
            if (this.App != context.App)
                throw new InvalidOperationException($"this.{nameof(App)} should be the same as {nameof(context)}.{nameof(context.App)}.");

            this.IsInitialized = true;
        }

        protected override void Uninitialize()
        {
            if (this.App is null)
                throw new InvalidOperationException($"this.{nameof(App)} should not be null.");

            this.IsUninitialized = true;
        }
    }
}
