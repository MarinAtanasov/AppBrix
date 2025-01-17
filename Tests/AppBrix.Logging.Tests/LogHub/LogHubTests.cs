// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Contracts;
using AppBrix.Logging.Events;
using AppBrix.Testing;
using System;

namespace AppBrix.Logging.Tests.LogHub;

[TestClass]
public sealed class LogHubTests : TestsBase<LoggingModule>
{
    #region Test lifecycle
    protected override void Initialize() => this.App.Start();
    #endregion

    #region Tests
    [Test, Functional]
    public void TestUnsubscribedTrace()
    {
        Action<ILogEntry> handler = _ => throw new InvalidOperationException("handler should have been unsubscribed");
        var hub = this.App.GetLogHub();
        hub.Subscribe(handler);
        hub.Unsubscribe(handler);
        hub.Trace(string.Empty);
    }

    [Test, Functional]
    public void TestErrorLog()
    {
        var message = "Test message";
        var error = new ArgumentException("Test exception");
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            this.Assert(x.Message == message, "the error message same as the passed in error");
            this.Assert(x.Exception == error, "the error message same as the passed in error");
            this.Assert(x.Level == LogLevel.Error, "log level should be Error");
            var stringed = x.ToString()!;
            this.Assert(stringed.Contains(x.Exception!.Message), "ToString should include the exception");
            this.Assert(stringed.Contains(x.Message), "ToString should include the message");
            this.Assert(stringed.Contains(x.Level.ToString()), "ToString should include the level");
        });
        this.App.GetLogHub().Error(message, error);
        this.Assert(called, "the event should have been called");
    }

    [Test, Functional]
    public void TestDebugLog()
    {
        var message = "Test message";
        var error = new ArgumentException("Test exception");
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            this.Assert(x.Message == message, "the debug message is different than the passed in message");
            this.Assert(x.Exception == error, "the error message same as the passed in error");
            this.Assert(x.Level == LogLevel.Debug, "log level should be Debug");
            this.Assert(x.GetHashCode() == x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString()!;
            this.Assert(stringed.Contains(x.Exception!.Message), "ToString should include the exception");
            this.Assert(stringed.Contains(x.Message), "ToString should include the message");
            this.Assert(stringed.Contains(x.Level.ToString()), "ToString should include the level");
        });
        this.App.GetLogHub().Debug(message, error);
        this.Assert(called, "the event should have been called");
    }

    [Test, Functional]
    public void TestInfoLog()
    {
        var message = "Test message";
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            this.Assert(x.Message == message, "the info message is different than the passed in message");
            this.Assert(x.Level == LogLevel.Info, "log level should be Info");
            this.Assert(x.GetHashCode() == x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString()!;
            this.Assert(stringed.Contains(x.Message), "ToString should include the message");
            this.Assert(stringed.Contains(x.Level.ToString()), "ToString should include the level");
        });
        this.App.GetLogHub().Info(message);
        this.Assert(called, "the event should have been called");
    }

    [Test, Functional]
    public void TestWarningLog()
    {
        var message = "Test message";
        var error = new ArgumentException("Test exception");
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            this.Assert(x.Message == message, "the warning message is different than the passed in message");
            this.Assert(x.Exception == error, "the error message same as the passed in error");
            this.Assert(x.Level == LogLevel.Warning, "log level should be Warning");
            this.Assert(x.GetHashCode() == x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString()!;
            this.Assert(stringed.Contains(x.Exception!.Message), "ToString should include the exception");
            this.Assert(stringed.Contains(x.Message), "ToString should include the message");
            this.Assert(stringed.Contains(x.Level.ToString()), "ToString should include the level");
        });
        this.App.GetLogHub().Warning(message, error);
        this.Assert(called, "the event should have been called");
    }

    [Test, Functional]
    public void TestTraceLog()
    {
        var message = "Test message";
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            this.Assert(x.Message == message, "the trace message is different than the passed in message");
            this.Assert(x.Level == LogLevel.Trace, "log level should be Trace");
            this.Assert(x.GetHashCode() == x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString()!;
            this.Assert(stringed.Contains(x.Message), "ToString should include the message");
            this.Assert(stringed.Contains(x.Level.ToString()), "ToString should include the level");
        });
        this.App.GetLogHub().Trace(message);
        this.Assert(called, "the event should have been called");
    }

    [Test, Functional]
    public void TestDisabledLogging()
    {
        var message = "Test message";
        var called = false;
        this.App.ConfigService.GetLoggingConfig().LogLevel = LogLevel.None;
        this.App.GetEventHub().Subscribe<ILogEntry>(_ => { called = true; });
        this.App.GetLogHub().Trace(message);
        this.App.GetLogHub().Debug(message);
        this.App.GetLogHub().Info(message);
        this.App.GetLogHub().Warning(message);
        this.App.GetLogHub().Error(message);
        this.App.GetLogHub().Critical(message);
        this.Assert(called == false, "the event should not have been called");
    }

    [Test, Functional]
    public void TestCallerFile()
    {
        this.App.GetEventHub().Subscribe<ILogEntry>(x => this.Assert(x.CallerFile.EndsWith(nameof(LogHubTests) + ".cs"), "caller file should be set to current file"));
        this.App.GetLogHub().Warning("Test message");
    }

    [Test, Functional]
    public void TestCallerMemberName()
    {
        this.App.GetEventHub().Subscribe<ILogEntry>(x => this.Assert(x.CallerMember == nameof(LogHubTests.TestCallerMemberName), "member name should be the current function"));
        this.App.GetLogHub().Error("Test message");
    }

    [Test, Functional]
    public void TestThreadId()
    {
        this.App.GetEventHub().Subscribe<ILogEntry>(x => this.Assert(x.ThreadId == Environment.CurrentManagedThreadId, "thread id should be current thread id"));
        this.App.GetLogHub().Info("Test message");
    }

    [Test, Functional]
    public void TestTimeLogEntry()
    {
        var message = "Test message";
        var before = this.App.GetTime();
        var executed = DateTime.MinValue;
        this.App.GetEventHub().Subscribe<ILogEntry>(x => executed = x.Created);
        this.App.GetLogHub().Debug(message);
        this.Assert(executed >= before, "created date time should be greater than the time before creation");
        this.Assert(executed <= this.App.GetTime(), "created date time should be greater than the time after creation");
    }

    [Test, Performance]
    public void TestPerformanceLogging()
    {
        this.App.GetEventHub().Subscribe<ILogEntry>(_ => { });

        this.AssertPerformance(this.TestPerformanceLoggingInternal);
    }
    #endregion

    #region Private methods
    private void TestPerformanceLoggingInternal()
    {
        const int repeat = 7500;
        const string message = "Test message";
        var error = new ArgumentException("Test error");
        var called = 0;
        var logHub = this.App.GetLogHub();

        void Handler(ILogEntry x) { called++; }
        this.App.GetEventHub().Subscribe((Action<ILogEntry>)Handler);
        for (var i = 0; i < repeat; i++)
        {
            logHub.Critical(message, error);
            logHub.Error(message, error);
            logHub.Debug(message);
            logHub.Info(message);
            logHub.Trace(message);
            logHub.Warning(message, error);
        }

        this.Assert(called == repeat * 6, "the event should have been called");

        this.App.Reinitialize();
    }
    #endregion
}
