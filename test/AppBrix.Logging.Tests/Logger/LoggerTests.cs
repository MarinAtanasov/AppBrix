// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Events;
using AppBrix.Events.Async;
using AppBrix.Factory;
using AppBrix.Logging.Impl;
using AppBrix.Logging.Tests.Mocks;
using AppBrix.Tests;
using AppBrix.Tests.Mocks;
using AppBrix.Time;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Logging.Tests.Logger
{
    public class LoggerTests
    {
        #region Setup and cleanup
        public LoggerTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ContainerModule),
                typeof(EventsModule),
                typeof(AsyncEventsModule),
                typeof(FactoryModule),
                typeof(LoggingModule),
                typeof(TimeModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestSyncLogger()
        {
            var writer = new LogWriterMock();
            var logger = new SyncLogger(writer);
            writer.IsInitialized.Should().BeFalse("the writer should not initialize before the logger");
            writer.LoggedEntries.Any().Should().BeFalse("no entries should be logged before initialization");
            logger.Initialize(new InitializeContextMock(this.app));
            writer.IsInitialized.Should().BeTrue("the writer should be initialized during the logger's initialization");
            writer.LoggedEntries.Any().Should().BeFalse("no entries should be logged just after initialization");

            string message = "Message";
            Exception ex = new ArgumentException("Test");
            this.app.GetLog().Info("Message", ex);
            writer.LoggedEntries.Count().Should().Be(1, "writer should have 1 entry passed in to it");
            writer.LoggedEntries.Single().Message.Should().Be(message, "the logged message should be the same as the passed in message");
            writer.LoggedEntries.Single().Exception.Should().Be(ex, "the logged exception should be the same as the passed in exception");

            logger.Uninitialize();
            this.app.Stop();
            writer.IsInitialized.Should().BeFalse("the writer should be uninitialized during the logger's uninitialization");
        }

        [Fact]
        public void TestAsyncLogger()
        {
            var writer = new LogWriterMock();
            var logger = new AsyncLogger(writer);
            writer.IsInitialized.Should().BeFalse("the writer should not initialize before the logger");
            writer.LoggedEntries.Any().Should().BeFalse("no entries should be logged before initialization");
            logger.Initialize(new InitializeContextMock(this.app));
            writer.IsInitialized.Should().BeTrue("the writer should be initialized during the logger's initialization");
            writer.LoggedEntries.Any().Should().BeFalse("no entries should be logged just after initialization");
            logger.Uninitialize();
            writer.IsInitialized.Should().BeFalse("the writer should be uninitialized during the logger's uninitialization");
            logger.Initialize(new InitializeContextMock(this.app));
            writer.IsInitialized.Should().BeTrue("the writer should be initialized during the logger's reinitialization");

            string message = "Message";
            Exception ex = new ArgumentException("Test");
            this.app.GetLog().Info("Message", ex);
            this.app.Stop();
            var entries = writer.LoggedEntries.ToList();
            entries.Count.Should().Be(1, "writer should have 1 entry passed in to it");
            entries[0].Message.Should().Be(message, "the logged message should be the same as the passed in message");
            entries[0].Exception.Should().Be(ex, "the logged exception should be the same as the passed in exception");
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
