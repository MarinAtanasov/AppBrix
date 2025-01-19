// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Testing;

/// <summary>
/// Defines commonly used test traits and categories.
/// </summary>
public static class TestCategories
{
    /// <summary>
    /// Constant for storing the test category trait.
    /// </summary>
    public const string Category = nameof(TestCategories.Category);

    /// <summary>
    /// Functional test category.
    /// </summary>
    public const string Functional = nameof(TestCategories.Functional);

    /// <summary>
    /// Performance test category.
    /// </summary>
    public const string Performance = nameof(TestCategories.Performance);
}
