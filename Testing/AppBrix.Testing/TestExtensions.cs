// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using FluentAssertions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AppBrix;

/// <summary>
/// Contains commonly used testing utilities.
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Checks that the action is executed under a specified time.
    /// It does one initial pass to make sure that the code path is loaded and hot.
    /// </summary>
    /// <param name="action">The action to be invoked.</param>
    /// <param name="duration">The maximum allowed duration.</param>
    /// <param name="firstPass">The maximum allowed duration on the first pass.</param>
    public static void AssertPerformance(this Action action, TimeSpan duration = default, TimeSpan firstPass = default)
    {
        if (duration == default)
            duration = TimeSpan.FromMilliseconds(100);
        if (firstPass == default)
            firstPass = TimeSpan.FromMilliseconds(5000);

        action.ExecutionTime().Should().BeLessThan(firstPass, "this is a performance test first pass");

        GC.Collect();

        action.ExecutionTime().Should().BeLessThan(duration, "this is a performance test");
    }

    /// <summary>
    /// Checks that the function is executed under a specified time.
    /// </summary>
    /// <param name="func">The function to be invoked.</param>
    /// <param name="duration">The maximum allowed duration.</param>
    /// <param name="firstPass">The maximum allowed duration on the first pass.</param>
    public static void AssertPerformance(this Func<Task> func, TimeSpan duration = default, TimeSpan firstPass = default)
    {
        var action = () => func().GetAwaiter().GetResult();
        action.AssertPerformance(duration, firstPass);
    }

    /// <summary>
    /// Asserts that a function will return a predefined value by a certain timeout.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="func">The function to be called until it returns the predefined value or the timeout expires.</param>
    /// <param name="result">The result which should be returned by the function before the timeout expires.</param>
    /// <param name="because">A formatted phrase explaining why the assertion should be satisfied. If the phrase does not start with the word because, it is prepended to the message.</param>
    /// <param name="becauseArgs">Zero or more values to use for filling in any <see cref="string.Format(string, object[])"/> compatible placeholders.</param>
    /// <returns>A task that completes when the function returns the predefined value</returns>
    public static Task ShouldReturn<T>(this Func<T> func, T result, string because = "", params object[] becauseArgs) =>
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
    /// <returns>A task that completes when the function returns the predefined value</returns>
    private static async Task ShouldReturn<T>(this Func<T> func, T result, TimeSpan timeout, string because = "", params object[] becauseArgs)
    {
        if (func is null)
            throw new ArgumentNullException(nameof(func));

        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            if (object.Equals(func(), result))
                return;

            await Task.Delay(1);
        }

        func().Should().Be(result, because, becauseArgs);
    }
}
