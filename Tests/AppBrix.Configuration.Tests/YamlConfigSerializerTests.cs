// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Yaml;

namespace AppBrix.Configuration.Tests;

public sealed class YamlConfigSerializerTests : ConfigSerializerTestsBase
{
    #region Protected methods
    protected override IConfigSerializer GetSerializer() => new YamlConfigSerializer();
    #endregion
}
