// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory.Impl;

namespace AppBrix.Configuration.Memory;

/// <summary>
/// In-memory implementation of the <see cref="IConfigService"/>.
/// </summary>
public sealed class MemoryConfigService : ConfigService
{
    /// <summary>
    /// Creates a new instance of <see cref="MemoryConfigService"/>.
    /// </summary>
    public MemoryConfigService() : base(new MemoryConfigProvider())
    {
    }
}
