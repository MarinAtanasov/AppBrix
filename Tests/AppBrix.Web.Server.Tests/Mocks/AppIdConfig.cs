// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using System;

namespace AppBrix.Web.Server.Tests.Mocks;

public class AppIdConfig : IConfig
{
	public Guid Id { get; set; }
}
