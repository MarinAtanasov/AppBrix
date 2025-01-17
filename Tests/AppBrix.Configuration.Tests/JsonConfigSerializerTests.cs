// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Json;
using AppBrix.Testing;

namespace AppBrix.Configuration.Tests;

[TestClass]
public sealed class JsonConfigSerializerTests : ConfigSerializerTestsBase
{
    #region Protected methods
    protected override IConfigSerializer GetSerializer() => new JsonConfigSerializer();
    #endregion
}
