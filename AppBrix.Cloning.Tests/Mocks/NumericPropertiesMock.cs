// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Cloning.Tests.Mocks
{
    internal class NumericPropertiesMock
    {
        #region Construction
        public NumericPropertiesMock()
        {
        }

        public NumericPropertiesMock(byte b, short s, int i, long l, float f, double d, decimal dec)
        {
        }
        #endregion

        #region Properties
        public byte Byte { get; set; }

        public short Short { get; set; }

        public int Int { get { return this.i; } }

        public long Long { get; private set; }

        public float Float { get { return this.f; } }

        public double Double { get; set; }

        public decimal Decimal { get; private set; }
        #endregion

        #region Private fields and constants
        private readonly int i = 0;
        private float f = 0;
        #endregion
    }
}
