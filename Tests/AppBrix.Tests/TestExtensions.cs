// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using FluentAssertions;
using System;
using System.Diagnostics;
using System.Threading;

namespace AppBrix.Tests;

/// <summary>
/// Contains commonly used testing utilities.
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Asserts that a function will return a predefined value by a certain timeout.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="func">The function to be called until it returns the predefined value or the timeout expires.</param>
    /// <param name="result">The result which should be returned by the function before the timeout expires.</param>
    /// <param name="because">A formatted phrase explaining why the assertion should be satisfied. If the phrase does not start with the word because, it is prepended to the message.</param>
    /// <param name="becauseArgs">Zero or more values to use for filling in any <see cref="string.Format(string, object[])"/> compatible placeholders.</param>
    public static void ShouldReturn<T>(this Func<T> func, T result, string because = "", params object[] becauseArgs) =>
        func.ShouldReturn(result, TimeSpan.FromSeconds(15), because, becauseArgs);

    /// <summary>
    /// Asserts that a function will return a predefined value by a certain timeout.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="func">The function to be called until it returns the predefined value or the timeout expires.</param>
    /// <param name="result">The result which should be returned by the function before the timeout expires.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="because">A formatted phrase explaining why the assertion should be satisfied. If the phrase does not start with the word because, it is prepended to the message.</param>
    /// <param name="becauseArgs">Zero or more values to use for filling in any <see cref="string.Format(string, object[])"/> compatible placeholders.</param>
    private static void ShouldReturn<T>(this Func<T> func, T result, TimeSpan timeout, string because = "", params object[] becauseArgs)
    {
        if (func is null)
            throw new ArgumentNullException(nameof(func));

        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            if (object.Equals(func(), result))
                return;

            Thread.Sleep(1);
        }

        func().Should().Be(result, because, becauseArgs);
    }
}
