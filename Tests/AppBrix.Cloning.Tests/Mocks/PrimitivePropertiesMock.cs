// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Cloning.Tests.Mocks;

internal sealed class PrimitivePropertiesMock : NumericPropertiesMock
{
	#region Construction
	public PrimitivePropertiesMock(bool b, char c, string s, DateTime d, TimeSpan t)
		: base(1, 2, 3, 4, 5.5f, 6.6, (decimal)7.7)
	{
		this.Bool = b;
		this.Char = c;
		this.String = s;
		this.DateTime = d;
		this.TimeSpan = t;
	}
	#endregion

	#region Properties
	public bool Bool { get; set; }

	public char Char { get; }

	public string String { get; private set; }

	public DateTime DateTime { get; set; }

	public TimeSpan TimeSpan { get; internal set; }
	#endregion
}
