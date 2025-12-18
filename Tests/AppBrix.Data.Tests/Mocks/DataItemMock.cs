// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Data.Tests.Mocks;

/// <summary>
/// A data item used during tests.
/// </summary>
public sealed class DataItemMock
{
	/// <summary>
	/// Gets or sets the id of the item.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the content of the item.
	/// </summary>
	public string Content { get; set; }
}
