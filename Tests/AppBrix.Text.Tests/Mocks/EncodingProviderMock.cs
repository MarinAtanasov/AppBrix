// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System.Text;

namespace AppBrix.Text.Tests.Mocks;

internal sealed class EncodingProviderMock : EncodingProvider
{
	#region Construction
	public EncodingProviderMock(Encoding encoding)
	{
		this.Encoding = encoding;
	}
	#endregion

	#region Properties
	public Encoding Encoding { get; }

	public bool IsGetEncodingWithNameCalled { get; private set; }

	public bool IsGetEncodingWithCodePageCalled { get; private set; }
	#endregion

	#region Public and overriden methods
	public override Encoding GetEncoding(string name)
	{
		this.IsGetEncodingWithNameCalled = true;
		return this.Encoding;
	}

	public override Encoding GetEncoding(int codepage)
	{
		this.IsGetEncodingWithCodePageCalled = true;
		return this.Encoding;
	}
	#endregion
}
