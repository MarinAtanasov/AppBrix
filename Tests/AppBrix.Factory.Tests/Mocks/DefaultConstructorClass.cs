// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Factory.Tests.Mocks
{
    internal sealed class DefaultConstructorClass
    {
        public DefaultConstructorClass()
        {
            this.ConstructorCalled = true;
        }

        public bool ConstructorCalled { get; private set; }
    }
}
