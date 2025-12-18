// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Web.Server.Tests.Mocks;

/// <summary>
/// A simple model which can be used to test sending and receiving data.
/// </summary>
public sealed class EchoModel
{
	public DateTime? DateTime { get; set; }

	public TimeSpan ?TimeSpan { get; set; }

	public int Value { get; set; }

	public Version Version { get; set; }

	public override bool Equals(object obj) => obj is EchoModel other &&
		this.DateTime == other.DateTime &&
		this.TimeSpan == other.TimeSpan &&
		this.Value == other.Value &&
		this.Version == other.Version;

	public override int GetHashCode() => 0;
}
