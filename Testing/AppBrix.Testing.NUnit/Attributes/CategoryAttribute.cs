﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with a test category.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CategoryAttribute : NUnit.Framework.CategoryAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="CategoryAttribute"/>.
    /// </summary>
    /// <param name="name">Must be <see cref="TestCategories.Category"/> for consistency with other providers.</param>
    /// <param name="value">The name of the category.</param>
    public CategoryAttribute(string name, string value) : base(value)
    {
    }
}
