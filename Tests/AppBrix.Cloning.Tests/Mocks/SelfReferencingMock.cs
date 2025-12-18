// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Cloning.Tests.Mocks;

internal sealed class SelfReferencingMock
{
	public SelfReferencingMock Other { get; set; }
}
