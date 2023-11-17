// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace AppBrix.Configuration.Tests.Mocks;

internal sealed class ConfigProviderMock : IConfigProvider
{
    #region Properties
    public IList<Type> ReadConfigs { get; } = new List<Type>();

    public IList<KeyValuePair<Type, IConfig>> WrittenConfigs { get; } = new List<KeyValuePair<Type, IConfig>>();
    #endregion

    #region Public and overriden methods
    public IConfig Get(Type config)
    {
        this.ReadConfigs.Add(config);
        return null;
    }

    public void Save(IConfig config) => 
        this.WrittenConfigs.Add(new KeyValuePair<Type, IConfig>(config.GetType(), config));

    public void Save(IEnumerable<IConfig> configs)
    {
        foreach (var config in configs)
        {
            this.Save(config);
        }
    }
    #endregion

}
