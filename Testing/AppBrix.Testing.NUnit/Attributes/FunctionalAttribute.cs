// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with the <see cref="AppBrix.Testing.TestCategories.Functional"/> category.
/// </summary>
public sealed class FunctionalAttribute : TestCategoryAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="FunctionalAttribute"/>.
    /// </summary>
    public FunctionalAttribute() : base(TestCategories.Category, TestCategories.Functional)
    {
    }
}
