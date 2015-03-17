// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Configuration.Memory;
using AppBrix.Events;
using AppBrix.Logging.Loggers;
using AppBrix.Logging.Tests.Mocks;
using AppBrix.Resolve;
using AppBrix.Tests;
using AppBrix.Tests.Mocks;
using AppBrix.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AppBrix.Logging.Tests.Logger
{
    [TestClass]
    public class LoggerTests
    {
        #region Setup and cleanup
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            LoggerTests.app = TestsUtils.CreateTestApp(
                typeof(ResolverModule),
                typeof(MemoryConfigModule),
                typeof(EventsModule),
                typeof(TimeModule),
                typeof(LoggingModule));
            LoggerTests.app.Start();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            LoggerTests.app.Stop();
            LoggerTests.app = null;
        }
        #endregion

        #region Tests
        [TestCleanup]
        public void Cleanup()
        {
            LoggerTests.app.Reinitialize();
        }

        [TestMethod]
        public void TestSyncLogger()
        {
            var writer = new LogWriterMock();
            var logger = new SyncLogger(writer);
            Assert.IsFalse(writer.IsInitialized, "The writer should not initialize before the logger.");
            Assert.IsFalse(writer.LoggedEntries.Any(), "No entries should be logged before initialization.");
            logger.Initialize(new InitializeContextMock(LoggerTests.app));
            Assert.IsTrue(writer.IsInitialized, "The writer should be initialized during the logger's initialization.");
            Assert.IsFalse(writer.LoggedEntries.Any(), "No entries should be logged just after initialization.");

            string message = "Message";
            Exception ex = new ArgumentException("Test");
            LoggerTests.app.GetLog().Info("Message", ex);
            Assert.AreEqual(1, writer.LoggedEntries.Count(), "Writer should have 1 entry passed in to it.");
            Assert.AreEqual(message, writer.LoggedEntries.Single().Message,
                "The logged message should be the same as the passed in message");
            Assert.AreEqual(ex, writer.LoggedEntries.Single().Error,
                "The logged exception should be the same as the passed in exception.");

            logger.Uninitialize();
            Assert.IsFalse(writer.IsInitialized, "The writer should be uninitialized during the logger's uninitialization.");
        }

        [TestMethod]
        public void TestAsyncSyncLogger()
        {
            var writer = new LogWriterMock();
            var logger = new AsyncLogger(writer);
            Assert.IsFalse(writer.IsInitialized, "The writer should not initialize before the logger.");
            Assert.IsFalse(writer.LoggedEntries.Any(), "No entries should be logged before initialization.");
            logger.Initialize(new InitializeContextMock(LoggerTests.app));
            Assert.IsTrue(writer.IsInitialized, "The writer should be initialized during the logger's initialization.");
            Assert.IsFalse(writer.LoggedEntries.Any(), "No entries should be logged just after initialization.");

            string message = "Message";
            Exception ex = new ArgumentException("Test");
            LoggerTests.app.GetLog().Info("Message", ex);
            logger.Uninitialize();
            var entries = writer.LoggedEntries.ToList();
            Assert.AreEqual(1, entries.Count, "Writer should have 1 entry passed in to it.");
            Assert.AreEqual(message, entries[0].Message,
                "The logged message should be the same as the passed in message");
            Assert.AreEqual(ex, entries[0].Error,
                "The logged exception should be the same as the passed in exception.");
            Assert.IsFalse(writer.IsInitialized, "The writer should be uninitialized during the logger's uninitialization.");
        }
        #endregion

        #region Private fields and constants
        private static IApp app;
        #endregion
    }
}
