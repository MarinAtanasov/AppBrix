// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with the <see cref="AppBrix.Testing.TestCategories.Functional"/> category.
/// </summary>
public sealed class FunctionalAttribute : CategoryAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="FunctionalAttribute"/>.
    /// Requires two parameters so that Xunit can detect the category.
    /// No need to explicitly provide values.
    /// </summary>
    /// <param name="name"><see cref="TestCategories.Category"/></param>
    /// <param name="value"><see cref="TestCategories.Functional"/></param>
    public FunctionalAttribute(string name = TestCategories.Category, string value = TestCategories.Functional) : base(name, value)
    {
    }
}
