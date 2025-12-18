// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Cloning.Tests.Mocks;

internal class NumericPropertiesMock
{
	#region Construction
	public NumericPropertiesMock()
	{
	}

	public NumericPropertiesMock(byte b, short s, int i, long l, float f, double d, decimal dec)
	{
		this.Byte = b;
		this.Short = s;
		this.Int = i;
		this.Long = l;
		this.f = f;
		this.Double = d;
		this.Decimal = dec;
	}
	#endregion

	#region Properties
	public byte Byte { get; set; }

	public short Short { get; set; }

	public int Int { get; }

	public long Long { get; private set; }

	public float Float => this.f;

	public double Double { get; protected set; }

	public decimal Decimal { get; private set; }
	#endregion

	#region Private fields and constants
	public float f;
	#endregion
}
