// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Permissions;
using System;
using System.Collections.Generic;

namespace AppBrix
{
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

        internal static void AddValue(this Dictionary<string, HashSet<string>> dictionary, string key, string value)
        {
            if (!dictionary.TryGetValue(key, out var values))
            {
                values = new HashSet<string>();
                dictionary.Add(key, values);
            }

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
            dictionary.TryGetValue(key, out var values) ? (IReadOnlyCollection<string>)values : Array.Empty<string>();
    }
}
