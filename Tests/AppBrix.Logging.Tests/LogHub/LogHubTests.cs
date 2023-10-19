// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Contracts;
using AppBrix.Logging.Events;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Logging.Tests.LogHub;

public sealed class LogHubTests : TestsBase<LoggingModule>
{
    #region Setup and cleanup
    public LogHubTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsubscribedTrace()
    {
        Action<ILogEntry> handler = _ => throw new InvalidOperationException("handler should have been unsubscribed");
        var hub = this.App.GetLogHub();
        hub.Subscribe(handler);
        hub.Unsubscribe(handler);
        hub.Trace(string.Empty);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestErrorLog()
    {
        var message = "Test message";
        var error = new ArgumentException("Test exception");
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            x.Message.Should().Be(message, "the error message same as the passed in error");
            x.Exception.Should().Be(error, "the error message same as the passed in error");
            x.Level.Should().Be(LogLevel.Error, "log level should be Error");
            var stringed = x.ToString();
            stringed.Should().Contain(x.Exception!.Message, "ToString should include the exception");
            stringed.Should().Contain(x.Message, "ToString should include the message");
            stringed.Should().Contain(x.Level.ToString(), "ToString should include the level");
        });
        this.App.GetLogHub().Error(message, error);
        called.Should().BeTrue("the event should have been called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDebugLog()
    {
        var message = "Test message";
        var error = new ArgumentException("Test exception");
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            x.Message.Should().Be(message, "the debug message is different than the passed in message");
            x.Exception.Should().Be(error, "the error message same as the passed in error");
            x.Level.Should().Be(LogLevel.Debug, "log level should be Debug");
            x.GetHashCode().Should().Be(x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString();
            stringed.Should().Contain(x.Exception!.Message, "ToString should include the exception");
            stringed.Should().Contain(x.Message, "ToString should include the message");
            stringed.Should().Contain(x.Level.ToString(), "ToString should include the level");
        });
        this.App.GetLogHub().Debug(message, error);
        called.Should().BeTrue("the event should have been called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInfoLog()
    {
        var message = "Test message";
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            x.Message.Should().Be(message, "the info message is different than the passed in message");
            x.Level.Should().Be(LogLevel.Info, "log level should be Info");
            x.GetHashCode().Should().Be(x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString();
            stringed.Should().Contain(x.Message, "ToString should include the message");
            stringed.Should().Contain(x.Level.ToString(), "ToString should include the level");
        });
        this.App.GetLogHub().Info(message);
        called.Should().BeTrue("the event should have been called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestWarningLog()
    {
        var message = "Test message";
        var error = new ArgumentException("Test exception");
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            x.Message.Should().Be(message, "the warning message is different than the passed in message");
            x.Exception.Should().Be(error, "the error message same as the passed in error");
            x.Level.Should().Be(LogLevel.Warning, "log level should be Warning");
            x.GetHashCode().Should().Be(x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString();
            stringed.Should().Contain(x.Exception!.Message, "ToString should include the exception");
            stringed.Should().Contain(x.Message, "ToString should include the message");
            stringed.Should().Contain(x.Level.ToString(), "ToString should include the level");
        });
        this.App.GetLogHub().Warning(message, error);
        called.Should().BeTrue("the event should have been called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestTraceLog()
    {
        var message = "Test message";
        var called = false;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
        {
            called = true;
            x.Message.Should().Be(message, "the trace message is different than the passed in message");
            x.Level.Should().Be(LogLevel.Trace, "log level should be Trace");
            x.GetHashCode().Should().Be(x.Message.GetHashCode(), "hash code should be the same as the message's hash code");
            var stringed = x.ToString();
            stringed.Should().Contain(x.Message, "ToString should include the message");
            stringed.Should().Contain(x.Level.ToString(), "ToString should include the level");
        });
        this.App.GetLogHub().Trace(message);
        called.Should().BeTrue("the event should have been called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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
        called.Should().BeFalse("the event should not have been called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestCallerFile()
    {
        var message = "Test message";
        this.App.GetEventHub().Subscribe<ILogEntry>(x => x.CallerFile.Should().EndWith(nameof(LogHubTests) + ".cs", "caller file should be set to current file"));
        this.App.GetLogHub().Warning(message);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestCallerMemberName()
    {
        var message = "Test message";
        this.App.GetEventHub().Subscribe<ILogEntry>(x => x.CallerMember.Should().Be("TestCallerMemberName", "member name should be the current function"));
        this.App.GetLogHub().Error(message);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestThreadId()
    {
        var message = "Test message";
        this.App.GetEventHub().Subscribe<ILogEntry>(x => x.ThreadId.Should().Be(Environment.CurrentManagedThreadId, "thread id should be current thread id"));
        this.App.GetLogHub().Info(message);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestTimeLogEntry()
    {
        var message = "Test message";
        var before = this.App.GetTime();
        var executed = DateTime.MinValue;
        this.App.GetEventHub().Subscribe<ILogEntry>(x => executed = x.Created);
        this.App.GetLogHub().Debug(message);
        executed.Should().BeOnOrAfter(before, "created date time should be greater than the time before creation");
        executed.Should().BeOnOrBefore(this.App.GetTime(), "created date time should be greater than the time after creation");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
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

        called.Should().Be(repeat * 6, "the event should have been called");

        this.App.Reinitialize();
    }
    #endregion
}
