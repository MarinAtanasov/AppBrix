// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBrix.Tests.Mocks
{
    public class InitializeContextMock : IInitializeContext
    {
        public InitializeContextMock(IApp app)
        {
            this.App = app;
        }

        public IApp App { get; private set; }
    }
}
