// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Configuration.Tests.Mocks;

internal sealed class ConfigSerializerMock : IConfigSerializer
{
    #region Public and overriden methods
    public string Serialize(object config) => config.GetType().FullName + " Serialized";

    public object Deserialize(string config, Type type) => null!;
    #endregion
}
