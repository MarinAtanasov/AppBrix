// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Defines a configuration manager for module level configuration.
    /// </summary>
    public interface IConfig
    {
        T GetSection<T>() where T : ConfigurationSection;
    }
}
