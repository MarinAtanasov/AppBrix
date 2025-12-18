// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Data.InMemory;
using AppBrix.Data.InMemory.Configuration;

namespace AppBrix;

/// <summary>
/// Extension methods for the <see cref="InMemoryDataModule"/>.
/// </summary>
public static class InMemoryDataExtensions
{
	/// <summary>
	/// Gets the <see cref="InMemoryDataConfig"/> from <see cref="IConfigService"/>.
	/// </summary>
	/// <param name="service">The configuration service.</param>
	/// <returns>The <see cref="InMemoryDataConfig"/>.</returns>
	public static InMemoryDataConfig GetInMemoryDataConfig(this IConfigService service) => (InMemoryDataConfig)service.Get(typeof(InMemoryDataConfig));
}
