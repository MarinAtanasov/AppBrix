﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using NUnit.Framework;
using System;

namespace AppBrix.Testing;

/// <summary>
/// Marks a test with the <see cref="AppBrix.Testing.TestCategories.Performance"/> category.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PerformanceAttribute : CategoryAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="PerformanceAttribute"/>.
    /// </summary>
    public PerformanceAttribute() : base(TestCategories.Performance)
    {
    }
}
