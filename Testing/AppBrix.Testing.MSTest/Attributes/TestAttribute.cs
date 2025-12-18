// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Testing;

/// <summary>
/// Marks a method as a test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestAttribute : Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
{
	/// <summary>
	/// Creates a new instance of <see cref="TestAttribute"/>.
	/// </summary>
	/// <param name="callerFilePath">Full path to the caller's file. Automatically filled.</param>
	/// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
	public TestAttribute(
		[System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "",
		[System.Runtime.CompilerServices.CallerLineNumber] int callerLineNumber = -1)
		: base(callerFilePath, callerLineNumber)
	{
	}
}
