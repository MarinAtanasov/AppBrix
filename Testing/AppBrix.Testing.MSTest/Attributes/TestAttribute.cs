// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AppBrix.Testing;

/// <summary>
/// Marks a method as a test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestAttribute : TestMethodAttribute;
