// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Logging.Tests.Logger;

public sealed class LoggerTests : TestsBase<LoggingModule>
{
    #region Setup and cleanup
    public LoggerTests() => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSyncLogger()
    {
        this.app.ConfigService.GetLoggingConfig().Async = false;
        var logger = new LoggerMock();
        logger.LoggedEntries.Any().Should().BeFalse("no entries should be logged before initialization");
        this.app.GetLogHub().Subscribe(logger.LogEntry);
        logger.LoggedEntries.Any().Should().BeFalse("no entries should be logged just after initialization");

        var message = "Message";
        Exception ex = new ArgumentException("Test");
        this.app.GetLogHub().Info(message, ex);
        logger.LoggedEntries.Count().Should().Be(1, "writer should have 1 entry passed in to it");
        logger.LoggedEntries.Single().Message.Should().Be(message, "the logged message should be the same as the passed in message");
        logger.LoggedEntries.Single().Exception.Should().Be(ex, "the logged exception should be the same as the passed in exception");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAsyncLogger()
    {
        this.app.ConfigService.GetLoggingConfig().Async = true;
        var logger = new LoggerMock();
        logger.LoggedEntries.Any().Should().BeFalse("no entries should be logged before initialization");
        this.app.GetLogHub().Subscribe(logger.LogEntry);
        logger.LoggedEntries.Any().Should().BeFalse("no entries should be logged just after initialization");

        var message = "Message";
        Exception ex = new ArgumentException("Test");
        this.app.GetLogHub().Info(message, ex);
        this.app.Restart();
        var entries = logger.LoggedEntries.ToList();
        entries.Count.Should().Be(1, "writer should have 1 entry passed in to it");
        entries[0].Message.Should().Be(message, "the logged message should be the same as the passed in message");
        entries[0].Exception.Should().Be(ex, "the logged exception should be the same as the passed in exception");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestLoggerLog()
    {
        this.app.ConfigService.GetLoggingConfig().Async = false;

        var expected = Contracts.LogLevel.None;
        var testLogLevel = (Contracts.LogLevel expectedLevel, LogLevel level) =>
        {
            expected = expectedLevel;
            this.app.Get<ILoggerProvider>().CreateLogger("test").Log(level, "");
        };

        this.app.GetLogHub().Subscribe(x => x.Level.Should().Be(expected, "the logger should map the levels correctly"));

        testLogLevel(Contracts.LogLevel.Trace, LogLevel.Trace);
        testLogLevel(Contracts.LogLevel.Debug, LogLevel.Debug);
        testLogLevel(Contracts.LogLevel.Info, LogLevel.Information);
        testLogLevel(Contracts.LogLevel.Warning, LogLevel.Warning);
        testLogLevel(Contracts.LogLevel.Error, LogLevel.Error);
        testLogLevel(Contracts.LogLevel.Critical, LogLevel.Critical);
        testLogLevel(Contracts.LogLevel.None, LogLevel.None);
    }
    #endregion
}
