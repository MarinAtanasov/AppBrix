// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Testing;

/// <summary>
/// A base framework-agnostic testing class.
/// </summary>
public abstract class TestingBase
{
    #region Test lifecycle
    /// <summary>
    /// Sets up the application.
    /// </summary>
    public virtual void Start(IApp app)
    {
        this.App = app;
    }

    /// <summary>
    /// Stops the application.
    /// </summary>
    public virtual void Stop()
    {
        this.Uninitialize();

        try { this.App?.Stop(); } catch (InvalidOperationException) { }
        this.App = null!;
    }

    /// <summary>
    /// Initialize the class before running a test.
    /// </summary>
    protected virtual void Initialize() { }

    /// <summary>
    /// Uninitializes the class after running a test.
    /// </summary>
    protected virtual void Uninitialize() { }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the app that is being tested.
    /// </summary>
    protected IApp App { get; private set; } = null!;
    #endregion

    #region Public and protected methods
    /// <summary>
    /// Asserts that the provided value is true.
    /// </summary>
    /// <param name="value">The value being asserted.</param>
    /// <param name="message">The error message if the assert fails.</param>
    /// <param name="expression">The calling expression of the value argument.</param>
    protected void Assert(bool value, string message = "", [CallerArgumentExpression(nameof(value))] string expression = "")
    {
        if (value)
            return;

        if (string.IsNullOrEmpty(message))
            throw new Exception($"Assertion failed: [{expression}] should be true.");

        throw new Exception($"Assertion failed: [{expression}] should be true, because {message}.");
    }

    /// <summary>
    /// Checks that the action is executed under a specified time.
    /// It does one initial pass to make sure that the code path is loaded and hot.
    /// </summary>
    /// <param name="action">The action to be invoked.</param>
    /// <param name="duration">The maximum allowed duration.</param>
    /// <param name="firstPass">The maximum allowed duration on the first pass.</param>
    protected void AssertPerformance(Action action, TimeSpan duration = default, TimeSpan firstPass = default)
    {
        if (duration == TimeSpan.Zero)
            duration = TimeSpan.FromMilliseconds(100);
        if (firstPass == TimeSpan.Zero)
            firstPass = TimeSpan.FromMilliseconds(5000);

        this.AssertExecuteIn(action, firstPass, "this is a performance test first pass");

        GC.Collect();

        this.AssertExecuteIn(action, duration, "this is a performance test");
    }

    /// <summary>
    /// Checks that the function is executed under a specified time.
    /// </summary>
    /// <param name="func">The function to be invoked.</param>
    /// <param name="duration">The maximum allowed duration.</param>
    /// <param name="firstPass">The maximum allowed duration on the first pass.</param>
    protected void AssertPerformance(Func<Task> func, TimeSpan duration = default, TimeSpan firstPass = default)
    {
        var action = () => func().GetAwaiter().GetResult();
        this.AssertPerformance(action, duration, firstPass);
    }

    /// <summary>
    /// Asserts that a function will return a predefined value by a certain timeout.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="func">The function to be called until it returns the predefined value or the timeout expires.</param>
    /// <param name="value">The result which should be returned by the function before the timeout expires.</param>
    /// <param name="message">Explanation why the function will return the expected value.</param>
    /// <returns>A task that completes when the function returns the predefined value</returns>
    protected Task AssertReturns<T>(Func<T> func, T value, string message = "") =>
        this.AssertReturns(func, value, TimeSpan.FromSeconds(15), message);

    /// <summary>
    /// Asserts that a function will return a predefined value by a certain timeout.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="func">The function to be called until it returns the predefined value or the timeout expires.</param>
    /// <param name="value">The result which should be returned by the function before the timeout expires.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="message">Explanation why the function will return the expected value.</param>
    /// <returns>A task that completes when the function returns the predefined value</returns>
    protected async Task AssertReturns<T>(Func<T> func, T value, TimeSpan timeout, string message = "")
    {
        if (func is null)
            throw new ArgumentNullException(nameof(func));

        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            if (object.Equals(func(), value))
                return;

            await Task.Delay(1);
        }

        this.Assert(object.Equals(func(), value), message);
    }

    /// <summary>
    /// Asserts that an action throws a specific exception.
    /// </summary>
    /// <param name="action">The action being tested.</param>
    /// <param name="message">Explanation why the action should throw.</param>
    /// <param name="expression">The calling expression of the value argument.</param>
    /// <typeparam name="T">The type of exception to be thrown.</typeparam>
    protected void AssertThrows<T>(Action action, string message = "", [CallerArgumentExpression(nameof(action))] string expression = "")
        where T : Exception
    {
        try
        {
            action();

            if (string.IsNullOrEmpty(message))
                throw new Exception($"Assertion failed: [{expression}] should have thrown an exception.");

            throw new Exception($"Assertion failed: [{expression}] should have thrown an exception, because {message}.");
        }
        catch (T) { }
        catch (Exception e)
        {
            if (string.IsNullOrEmpty(message))
                throw new Exception($"Assertion failed: [{expression}] should have thrown an exception of type {typeof(T).FullName}, but received {e.GetType().FullName}.");

            throw new Exception($"Assertion failed: [{expression}] should have thrown an exception of type {typeof(T).FullName}, because {message}, but received {e.GetType().FullName}.");
        }
    }

    /// <summary>
    /// Asserts that a function throws a specific exception.
    /// </summary>
    /// <param name="func">The function being tested.</param>
    /// <param name="message">Explanation why the action should throw.</param>
    /// <param name="expression">The calling expression of the value argument.</param>
    /// <typeparam name="T">The type of exception to be thrown.</typeparam>
    protected void AssertThrows<T>(Func<object> func, string message = "", [CallerArgumentExpression(nameof(func))] string expression = "")
        where T : Exception
    {
        try
        {
            func();

            if (string.IsNullOrEmpty(message))
                throw new Exception($"Assertion failed: [{expression}] should have thrown an exception.");

            throw new Exception($"Assertion failed: [{expression}] should have thrown an exception, because {message}.");
        }
        catch (T) { }
        catch (Exception e)
        {
            if (string.IsNullOrEmpty(message))
                throw new Exception($"Assertion failed: [{expression}] should have thrown an exception of type {typeof(T).FullName}, but received {e.GetType().FullName}.");

            throw new Exception($"Assertion failed: [{expression}] should have thrown an exception of type {typeof(T).FullName}, because {message}, but received {e.GetType().FullName}.");
        }
    }
    #endregion

    #region Private methods
    private void AssertExecuteIn(Action action, TimeSpan limit, string message = "")
    {
        var task = Task.Run(action);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < limit)
        {
            if (task.IsCompleted)
            {
                if (task.IsCompletedSuccessfully)
                    return;

                if (string.IsNullOrEmpty(message))
                    throw new Exception($"Assertion failed: {nameof(action)} should have executed, but threw an exception.", task.Exception);

                throw new Exception($"Assertion failed: {nameof(action)} should have executed, because {message}, but threw an exception.", task.Exception);
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(1));
        }

        if (string.IsNullOrEmpty(message))
            throw new Exception($"Assertion failed: {nameof(action)} should have executed within the time limit.");

        throw new Exception($"Assertion failed: {nameof(action)} should have executed within the time limit, because {message}.");
    }
    #endregion
}
