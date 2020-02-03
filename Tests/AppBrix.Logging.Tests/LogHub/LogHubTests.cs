// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Entries;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Threading;
using Xunit;

namespace AppBrix.Logging.Tests.LogHub
{
    public sealed class LogHubTests : TestsBase
    {
        #region Setup and cleanup
        public LogHubTests() : base(TestUtils.CreateTestApp<LoggingModule>()) => this.app.Start();
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUnsubscribedTrace()
        {
            this.app.GetLogHub().Trace(string.Empty);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestErrorLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the error message same as the passed in error");
                x.Exception.Should().Be(error, "the error message same as the passed in error");
                x.Level.Should().Be(LogLevel.Error, "log level should be Error");
            });
            this.app.GetLogHub().Error(message, error);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestDebugLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the debug message is different than the passed in message");
                x.Exception.Should().Be(error, "the error message same as the passed in error");
                x.Level.Should().Be(LogLevel.Debug, "log level should be Debug");
            });
            this.app.GetLogHub().Debug(message, error);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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
            this.app.GetLogHub().Info(message);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestWarningLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                x.Message.Should().Be(message, "the warning message is different than the passed in message");
                x.Exception.Should().Be(error, "the error message same as the passed in error");
                x.Level.Should().Be(LogLevel.Warning, "log level should be Warning");
            });
            this.app.GetLogHub().Warning(message, error);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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
            this.app.GetLogHub().Trace(message);
            called.Should().BeTrue("the event should have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestCallerFile()
        {
            var message = "Test message";
            this.app.GetEventHub().Subscribe<ILogEntry>(x => x.CallerFile.Should().EndWith(typeof(LogHubTests).Name + ".cs", "caller file should be set to current file"));
            this.app.GetLogHub().Warning(message);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestCallerMemberName()
        {
            var message = "Test message";
            this.app.GetEventHub().Subscribe<ILogEntry>(x => x.CallerMember.Should().Be("TestCallerMemberName", "member name should be the current function"));
            this.app.GetLogHub().Error(message);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestThreadId()
        {
            var message = "Test message";
            this.app.GetEventHub().Subscribe<ILogEntry>(x => x.ThreadId.Should().Be(Thread.CurrentThread.ManagedThreadId, "thread id should be current thread id"));
            this.app.GetLogHub().Info(message);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestTimeLogEntry()
        {
            var message = "Test message";
            var before = this.app.GetTime();
            DateTime executed = DateTime.MinValue;
            this.app.GetEventHub().Subscribe<ILogEntry>(x => executed = x.Created);
            this.app.GetLogHub().Debug(message);
            executed.Should().BeOnOrAfter(before, "created date time should be greater than the time before creation");
            executed.Should().BeOnOrBefore(this.app.GetTime(), "created date time should be greater than the time after creation");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceLogging()
        {
            this.app.GetEventHub().Subscribe<ILogEntry>(x => { });

            TestUtils.TestPerformance(this.TestPerformanceLoggingInternal);
        }
        #endregion

        #region Private methods
        private void TestPerformanceLoggingInternal()
        {
            var message = "Test message";
            var error = new ArgumentException("Test error");
            var called = 0;
            var repeat = 5000;
            Action<ILogEntry> handler = x => { called++; };
            this.app.GetEventHub().Subscribe(handler);
            var logHub = this.app.GetLogHub();
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
            this.app.Reinitialize();
        }
        #endregion
    }
}
