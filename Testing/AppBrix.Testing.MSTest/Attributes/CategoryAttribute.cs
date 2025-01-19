// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with a test category.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CategoryAttribute : Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="CategoryAttribute"/>.
    /// </summary>
    /// <param name="name">Must be <see cref="TestCategories.Category"/> for consistency with other providers.</param>
    /// <param name="value">The name of the category.</param>
    public CategoryAttribute(string name, string value)
    {
        this.TestCategories = [value];
    }

    /// <summary>
    /// Gets the test category that has been applied to the test.
    /// </summary>
    public override IList<string> TestCategories { get; }
}
