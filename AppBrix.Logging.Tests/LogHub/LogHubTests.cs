// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events;
using AppBrix.Logging.Entries;
using AppBrix.Resolver;
using AppBrix.Tests;
using AppBrix.Time;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace AppBrix.Logging.Tests.LogHub
{
    public class LogHubTests : IDisposable
    {
        #region Setup and cleanup
        public LogHubTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ResolverModule),
                typeof(EventsModule),
                typeof(TimeModule),
                typeof(LoggingModule));
            this.app.Start();
        }

        public void Dispose()
        {
            this.app.Stop();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestUnsubscribedTrace()
        {
            this.app.GetLog().Trace(string.Empty);
        }

        [Fact]
        public void TestErrorLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the error message same as the passed in error");
                x.Error.Should().Be(error, "the error message same as the passed in error");
                x.Level.Should().Be(LogLevel.Error, "log level should be Error");
            });
            this.app.GetLog().Error(message, error);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact]
        public void TestDebugLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the debug message is different than the passed in message");
                x.Error.Should().Be(error, "the error message same as the passed in error");
                x.Level.Should().Be(LogLevel.Debug, "log level should be Debug");
            });
            this.app.GetLog().Debug(message, error);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact]
        public void TestInfoLog()
        {
            var message = "Test message";
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the info message is different than the passed in message");
                x.Level.Should().Be(LogLevel.Info, "log level should be Info");
            });
            this.app.GetLog().Info(message);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact]
        public void TestWarningLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the warning message is different than the passed in message");
                x.Error.Should().Be(error, "the error message same as the passed in error");
                x.Level.Should().Be(LogLevel.Warning, "log level should be Warning");
            });
            this.app.GetLog().Warning(message, error);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact]
        public void TestTraceLog()
        {
            var message = "Test message";
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the trace message is different than the passed in message");
                x.Level.Should().Be(LogLevel.Trace, "log level should be Trace");
            });
            this.app.GetLog().Trace(message);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact]
        public void TestCallerFile()
        {
            var message = "Test message";
            this.app.GetEventHub().Subscribe<ILogEntry>(x => x.CallerFile.Should().EndWith(typeof(LogHubTests).Name + ".cs", "caller file should be set to current file"));
            this.app.GetLog().Warning(message);
        }

        [Fact]
        public void TestCallerMemberName()
        {
            var message = "Test message";
            this.app.GetEventHub().Subscribe<ILogEntry>(x => x.CallerMember.Should().Be("TestCallerMemberName", "member name should be the current function"));
            this.app.GetLog().Error(message);
        }

        [Fact]
        public void TestThreadId()
        {
            var message = "Test message";
            this.app.GetEventHub().Subscribe<ILogEntry>(x => x.ThreadId.Should().Be(Thread.CurrentThread.ManagedThreadId, "thread id should be current thread id"));
            this.app.GetLog().Info(message);
        }

        [Fact]
        public void TestTimeLogEntry()
        {
            var message = "Test message";
            var before = this.app.GetTime();
            DateTime executed = DateTime.MinValue;
            this.app.GetEventHub().Subscribe<ILogEntry>(x => executed = x.Created);
            this.app.GetLog().Debug(message);
            executed.Should().BeOnOrAfter(before, "created date time should be greater than the time before creation");
            executed.Should().BeOnOrBefore(this.app.GetTime(), "created date time should be greater than the time after creation");
        }

        [Fact]
        public void TestPerformanceLogging()
        {
            Action action = () => this.TestPerformanceLoggingInternal();
            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(20), "this is a performance test");
        }

        [Fact]
        public void TestPerformanceTraceLog()
        {
            Action action = () => this.TestPerformanceTraceLogInternal();
            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(20), "this is a performance test");
        }
        #endregion

        #region Private methods
        private void TestPerformanceLoggingInternal()
        {
            var message = "Test message";
            var error = new ArgumentException("Test error");
            var called = 0;
            var repeat = 100;
            Action<ILogEntry> handler = x => { called++; };
            this.app.GetEventHub().Subscribe<ILogEntry>(handler);
            for (int i = 0; i < repeat; i++)
            {
                this.app.GetLog().Critical(message, error);
                this.app.GetLog().Error(message, error);
                this.app.GetLog().Debug(message);
                this.app.GetLog().Info(message);
                this.app.GetLog().Trace(message);
                this.app.GetLog().Warning(message, error);
            }
            called.Should().Be(repeat * 6, "the event should have been called");
        }

        private void TestPerformanceTraceLogInternal()
        {
            var message = "Test message";
            var error = new ArgumentException("Test error");
            var called = 0;
            var repeat = 150;
            Action<ILogEntry> handler = x => { called++; };
            this.app.GetEventHub().Subscribe<ILogEntry>(handler);
            for (int i = 0; i < repeat; i++)
            {
                this.app.GetLog().Trace(message);
            }
            called.Should().Be(repeat, "the event should have been called");
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
