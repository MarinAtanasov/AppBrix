// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix.Tests.Mocks
{
    public sealed class MainModuleMock<T> : MainModuleBase where T : IModule
    {
        public override IEnumerable<Type> Dependencies => new[] { typeof(T) }; 
    }
    
    public sealed class MainModuleMock<T1, T2> : MainModuleBase where T1 : IModule where T2 : IModule
    {
        public override IEnumerable<Type> Dependencies => new[] { typeof(T1), typeof(T2) }; 
    }
}