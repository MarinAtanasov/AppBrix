// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Cloning.Tests.Mocks;

internal sealed class DelegateWrapperMock
{
	public Func<DelegateWrapperMock> Delegate { get; set; }
}
