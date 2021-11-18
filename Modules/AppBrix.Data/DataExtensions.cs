// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data;

namespace AppBrix;

/// <summary>
/// Extension methods for the <see cref="DataModule"/>.
/// </summary>
public static class DataExtensions
{
    /// <summary>
    /// Gets the application's currently registered <see cref="IDbContextService"/>
    /// </summary>
    /// <param name="app">The application.</param>
    /// <returns>The registered databse context service.</returns>
    public static IDbContextService GetDbContextService(this IApp app) => (IDbContextService)app.Get(typeof(IDbContextService));

    /// <summary>
    /// Gets the currently registered <see cref="IDbContextConfigurer"/>.
    /// </summary>
    /// <param name="app">The current <see cref="IApp"/>.</param>
    /// <returns>The registered context configurer.</returns>
    internal static IDbContextConfigurer GetDbContextConfigurer(this IApp app) => (IDbContextConfigurer)app.Get(typeof(IDbContextConfigurer));
}
