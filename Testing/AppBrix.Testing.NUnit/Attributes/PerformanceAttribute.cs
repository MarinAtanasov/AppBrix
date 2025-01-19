// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with the <see cref="AppBrix.Testing.TestCategories.Performance"/> category.
/// </summary>
public sealed class PerformanceAttribute : CategoryAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="PerformanceAttribute"/>.
    /// </summary>
    public PerformanceAttribute() : base(TestCategories.Category, TestCategories.Performance)
    {
    }
}
