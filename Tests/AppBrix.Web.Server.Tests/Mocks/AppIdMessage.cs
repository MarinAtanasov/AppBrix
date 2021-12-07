// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Web.Server.Tests.Mocks;

public sealed class AppIdMessage
{
    public TimeSpan Duration { get; set; }

    public Guid Id { get; set; }

    public DateTime Time { get; set; }

    public Version Version { get; set; }
}
