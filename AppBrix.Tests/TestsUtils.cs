// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix.Tests
{
    public static class TestsUtils
    {
        #region Public methods
        public static IApp CreateTestApp(params Type[] modules)
        {
            var config = new MemoryAppConfig();
            foreach (var module in modules)
            {
                config.Modules.Add(ModuleConfigElement.Create(module));
            }
            return new DefaultApp(config);
        }
        #endregion
    }
}
