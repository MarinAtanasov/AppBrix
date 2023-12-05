// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace AppBrix.Configuration.Memory.Impl;

internal sealed class MemoryConfigProvider : IConfigProvider
{
    #region Public and overriden methods
    public IConfig? Get(Type type) => null;

    public void Save(IConfig config) { }

    public void Save(IEnumerable<IConfig> configs) { }
    #endregion
}
