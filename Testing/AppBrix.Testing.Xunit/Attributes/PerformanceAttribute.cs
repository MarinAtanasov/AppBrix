// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with the <see cref="AppBrix.Testing.TestCategories.Performance"/> category.
/// </summary>
public sealed class PerformanceAttribute : TestCategoryAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="PerformanceAttribute"/>.
    /// Requires two parameters so that Xunit can detect the category.
    /// No need to explicitly provide values.
    /// </summary>
    /// <param name="name"><see cref="TestCategories.Category"/></param>
    /// <param name="value"><see cref="TestCategories.Performance"/></param>
    public PerformanceAttribute(string name = TestCategories.Category, string value = TestCategories.Performance) : base(name, value)
    {
    }
}
