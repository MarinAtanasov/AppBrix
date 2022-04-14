// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

using System;

namespace AppBrix.Configuration.Tests.Mocks;

internal sealed class ConfigMock : IConfig
{
    public DateTimeKind Enum { get; set; }

    public TimeSpan? TimeSpan { get; set; }

    public Version Version { get; set; }
}
