// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Tests.Mocks;
using AppBrix.Testing;
using Microsoft.Extensions.Logging;
using System;

namespace AppBrix.Logging.Tests.Logger;

[TestClass]
public sealed class LoggerTests : TestsBase<LoggingModule>
{
    #region Test lifecycle
    protected override void Initialize() => this.App.Start();
    #endregion

    #region Tests
    [Test, Functional]
    public void TestSyncLogger()
    {
        this.App.ConfigService.GetLoggingConfig().Async = false;
        var logger = new LoggerMock();
        this.Assert(logger.LoggedEntries.Count == 0, "no entries should be logged before initialization");
        this.App.GetLogHub().Subscribe(logger.LogEntry);
        this.Assert(logger.LoggedEntries.Count == 0, "no entries should be logged just after initialization");

        var message = "Message";
        Exception ex = new ArgumentException("Test");
        this.App.GetLogHub().Info(message, ex);
        this.Assert(logger.LoggedEntries.Count == 1, "writer should have 1 entry passed in to it");
        this.Assert(logger.LoggedEntries[0].Message == message, "the logged message should be the same as the passed in message");
        this.Assert(logger.LoggedEntries[0].Exception == ex, "the logged exception should be the same as the passed in exception");
    }

    [Test, Functional]
    public void TestAsyncLogger()
    {
        this.App.ConfigService.GetLoggingConfig().Async = true;
        var logger = new LoggerMock();
        this.Assert(logger.LoggedEntries.Count == 0, "no entries should be logged before initialization");
        this.App.GetLogHub().Subscribe(logger.LogEntry);
        this.Assert(logger.LoggedEntries.Count == 0,"no entries should be logged just after initialization");

        var message = "Message";
        Exception ex = new ArgumentException("Test");
        this.App.GetLogHub().Info(message, ex);
        this.App.Restart();
        this.Assert(logger.LoggedEntries.Count == 1, "writer should have 1 entry passed in to it");
        this.Assert(logger.LoggedEntries[0].Message == message, "the logged message should be the same as the passed in message");
        this.Assert(logger.LoggedEntries[0].Exception == ex, "the logged exception should be the same as the passed in exception");
    }

    [Test, Functional]
    public void TestLoggerLog()
    {
        this.App.ConfigService.GetLoggingConfig().Async = false;

        var expected = Contracts.LogLevel.None;
        var testLogLevel = (Contracts.LogLevel expectedLevel, LogLevel level) =>
        {
            expected = expectedLevel;
            this.App.Get<ILoggerProvider>().CreateLogger("test").Log(level, "");
        };

        this.App.GetLogHub().Subscribe(x => this.Assert(x.Level == expected, "the logger should map the levels correctly"));

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
