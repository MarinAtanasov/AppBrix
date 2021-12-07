// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory.Services;

namespace AppBrix;

/// <summary>
/// Extension methods for easier manipulation of AppBrix factories.
/// </summary>
public static class FactoryExtensions
{
    /// <summary>
    /// Gets the currently loaded factory service.
    /// </summary>
    /// <param name="app">The current application.</param>
    /// <returns>The factory.</returns>
    public static IFactoryService GetFactoryService(this IApp app) => (IFactoryService)app.Get(typeof(IFactoryService));
}
