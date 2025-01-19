// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Testing;

/// <summary>
/// Marks a class as a class containing test methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TestClassAttribute : NUnit.Framework.TestFixtureAttribute;
