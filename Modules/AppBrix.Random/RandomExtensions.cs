// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Random;
using AppBrix.Random.Services;

namespace AppBrix;

/// <summary>
/// Extension methods for the <see cref="RandomModule"/>.
/// </summary>
public static class RandomExtensions
{
	/// <summary>
	/// Gets the application's currently registered <see cref="IRandomService"/>
	/// </summary>
	/// <param name="app">The application.</param>
	/// <returns>The registered random service.</returns>
	public static IRandomService GetRandomService(this IApp app) => (IRandomService)app.Get(typeof(IRandomService));
}
