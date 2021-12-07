// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Logging.Tests.Logger;

public sealed class LoggerTests : TestsBase
{
    #region Setup and cleanup
    public LoggerTests() : base(TestUtils.CreateTestApp<LoggingModule>()) => this.app.Start();
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

        this.app.GetLogHub().Unsubscribe(logger.LogEntry);
        this.app.Stop();
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
        this.app.Stop();
        var entries = logger.LoggedEntries.ToList();
        entries.Count.Should().Be(1, "writer should have 1 entry passed in to it");
        entries[0].Message.Should().Be(message, "the logged message should be the same as the passed in message");
        entries[0].Exception.Should().Be(ex, "the logged exception should be the same as the passed in exception");
    }
    #endregion
}
