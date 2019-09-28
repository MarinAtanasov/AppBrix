// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;

namespace AppBrix.Cloning.Tests.Mocks
{
    internal sealed class ComplexPropertiesMock
    {
        public ComplexPropertiesMock(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                this.items.Add(i, (i % 2 == 0) ?
                    new NumericPropertiesMock((byte)i, (short)(i + 1), i + 2, i + 3, (float)(i * 1.1), i * 1.2, i * (decimal)1.3) :
                    new PrimitivePropertiesMock(i % 3 == 1, (char)i, new string((char)i, i + 1), i % 3 == 0 ? DateTime.Now : DateTime.UtcNow, TimeSpan.FromMilliseconds(i * i * 42)));
            }
        }

        private readonly Dictionary<int, NumericPropertiesMock> items = new Dictionary<int, NumericPropertiesMock>();
    }
}
