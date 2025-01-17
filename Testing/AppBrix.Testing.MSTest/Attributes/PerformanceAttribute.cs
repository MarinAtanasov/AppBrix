// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with the <see cref="AppBrix.Testing.TestCategories.Performance"/> category.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PerformanceAttribute : TestCategoryBaseAttribute
{
    /// <summary>
    /// Gets the test category that has been applied to the test.
    /// </summary>
    public override IList<string> TestCategories { get; } = [AppBrix.Testing.TestCategories.Performance];
}
