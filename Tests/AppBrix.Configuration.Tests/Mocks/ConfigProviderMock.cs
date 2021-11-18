// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;

namespace AppBrix.Configuration.Tests.Mocks;

internal sealed class ConfigProviderMock : IConfigProvider
{
    #region Properties
    public IList<Type> ReadConfigs { get; } = new List<Type>();

    public IList<KeyValuePair<Type, string>> WrittenConfigs { get; } = new List<KeyValuePair<Type, string>>();
    #endregion

    #region Public and overriden methods
    public string ReadConfig(Type type)
    {
        this.ReadConfigs.Add(type);
        return type.FullName + " Read";
    }

    public void WriteConfig(string config, Type type) => this.WrittenConfigs.Add(new KeyValuePair<Type, string>(type, config));
    #endregion
}
