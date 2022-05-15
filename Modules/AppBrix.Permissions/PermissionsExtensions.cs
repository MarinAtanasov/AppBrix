// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Permissions.Configuration;
using AppBrix.Permissions.Services;
using System;
using System.Collections.Generic;

namespace AppBrix;

/// <summary>
/// Extension methods for easier manipulation of AppBrix permissions.
/// </summary>
public static class PermissionsExtensions
{
    /// <summary>
    /// Gets the currently loaded permissions service.
    /// </summary>
    /// <param name="app">The current application.</param>
    /// <returns>The permissions service.</returns>
    public static IPermissionsService GetPermissionsService(this IApp app) => (IPermissionsService)app.Get(typeof(IPermissionsService));

    /// <summary>
    /// Gets the <see cref="PermissionsConfig"/> from <see cref="IConfigService"/>.
    /// </summary>
    /// <param name="service">The configuration service.</param>
    /// <returns>The <see cref="PermissionsConfig"/>.</returns>
    public static PermissionsConfig GetPermissionsConfig(this IConfigService service) => (PermissionsConfig)service.Get(typeof(PermissionsConfig));

    internal static void AddValue(this Dictionary<string, HashSet<string>> dictionary, string key, string value)
    {
        if (!dictionary.TryGetValue(key, out var values))
            dictionary[key] = values = new HashSet<string>();

        values.Add(value);
    }

    internal static void RemoveValue(this Dictionary<string, HashSet<string>> dictionary, string key, string value)
    {
        if (dictionary.TryGetValue(key, out var values))
        {
            if (values.Remove(value) && values.Count == 0)
            {
                dictionary.Remove(key);
            }
        }
    }

    internal static IReadOnlyCollection<string> GetOrEmpty(this Dictionary<string, HashSet<string>> dictionary, string key) =>
        dictionary.TryGetValue(key, out var values) ? values : Array.Empty<string>();
}
