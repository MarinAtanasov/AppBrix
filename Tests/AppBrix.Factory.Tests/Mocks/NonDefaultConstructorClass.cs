// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Factory.Tests.Mocks;

internal sealed class NonDefaultConstructorClass : DefaultConstructorClass, ITestInterface
{
    public NonDefaultConstructorClass(bool val)
    {
        this.Value = val;
    }

    public bool Modified { get; set; }

    public bool Value { get; }
}
