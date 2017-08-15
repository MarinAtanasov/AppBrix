// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace AppBrix.Caching.Tests.Mocks
{
    internal sealed class CustomMemoryDistributedCacheOptions : MemoryDistributedCacheOptions, IOptions<MemoryDistributedCacheOptions>
    {
        public MemoryDistributedCacheOptions Value => this;
    }
}
