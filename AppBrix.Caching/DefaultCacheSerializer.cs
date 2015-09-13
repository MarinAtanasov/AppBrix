// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Caching
{
    internal class DefaultCacheSerializer : ICacheSerializer
    {
        public object Deserialize(Type type, byte[] serialized)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize(Type type, object item)
        {
            throw new NotImplementedException();
        }
    }
}
