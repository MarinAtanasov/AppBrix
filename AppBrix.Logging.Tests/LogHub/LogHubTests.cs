// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration.Memory;
using AppBrix.Events;
using AppBrix.Logging.Entries;
using AppBrix.Resolver;
using AppBrix.Tests;
using AppBrix.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace AppBrix.Logging.Tests.LogHub
{
    [TestClass]
    public class LogHubTests
    {
        #region Setup and cleanup
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            LogHubTests.app = TestUtils.CreateTestApp(
                typeof(ResolverModule),
                typeof(MemoryConfigModule),
                typeof(EventsModule),
                typeof(TimeModule),
                typeof(LoggingModule));
            LogHubTests.app.Start();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            LogHubTests.app.Stop();
            LogHubTests.app = null;
        }

        [TestCleanup]
        public void Cleanup()
        {
            LogHubTests.app.Reinitialize();
        }
        #endregion

        #region Tests
        [TestMethod]
        public void TestUnsubscribedTrace()
        {
            LogHubTests.app.GetLog().Trace(string.Empty);
        }

        [TestMethod]
        public void TestErrorLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                Assert.AreEqual(message, x.Message, "Error message is different than the passed in message.");
                Assert.AreEqual(error, x.Error, "Error is different than the passed in error.");
                Assert.AreEqual(LogLevel.Error, x.Level, "Log level is not Error.");
            });
            LogHubTests.app.GetLog().Error(message, error);
            Assert.IsTrue(called, "The event has not been called.");
        }

        [TestMethod]
        public void TestDebugLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                Assert.AreEqual(message, x.Message, "Debug message is different than the passed in message.");
                Assert.AreEqual(error, x.Error, "Error is different than the passed in error.");
                Assert.AreEqual(LogLevel.Debug, x.Level, "Log level is not Debug.");
            });
            LogHubTests.app.GetLog().Debug(message, error);
            Assert.IsTrue(called, "The event has not been called.");
        }

        [TestMethod]
        public void TestInfoLog()
        {
            var message = "Test message";
            var called = false;
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                Assert.AreEqual(message, x.Message, "Info message is different than the passed in message.");
                Assert.AreEqual(LogLevel.Info, x.Level, "Log level is not Info.");
            });
            LogHubTests.app.GetLog().Info(message);
            Assert.IsTrue(called, "The event has not been called.");
        }

        [TestMethod]
        public void TestWarningLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test exception");
            var called = false;
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                Assert.AreEqual(message, x.Message, "Warning message is different than the passed in message.");
                Assert.AreEqual(error, x.Error, "Error is different than the passed in error.");
                Assert.AreEqual(LogLevel.Warning, x.Level, "Log level is not Warning.");
            });
            LogHubTests.app.GetLog().Warning(message, error);
            Assert.IsTrue(called, "The event has not been called.");
        }

        [TestMethod]
        public void TestTraceLog()
        {
            var message = "Test message";
            var called = false;
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                called = true;
                Assert.AreEqual(message, x.Message, "Trace message is different than the passed in message.");
                Assert.AreEqual(LogLevel.Trace, x.Level, "Log level is not Trace.");
            });
            LogHubTests.app.GetLog().Trace(message);
            Assert.IsTrue(called, "The event has not been called.");
        }

        [TestMethod]
        public void TestCallerFile()
        {
            var message = "Test message";
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x => Assert.IsTrue(x.CallerFile.EndsWith(typeof(LogHubTests).Name + ".cs"), "Incorrect file name."));
            LogHubTests.app.GetLog().Warning(message);
        }

        [TestMethod]
        public void TestCallerMemberName()
        {
            var message = "Test message";
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x => Assert.AreEqual("TestCallerMemberName", x.CallerMember, "Incorrect member name."));
            LogHubTests.app.GetLog().Error(message);
        }

        [TestMethod]
        public void TestThreadId()
        {
            var message = "Test message";
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x => Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, x.ThreadId, "Incorrect member name."));
            LogHubTests.app.GetLog().Info(message);
        }

        [TestMethod]
        public void TestTimeLogEntry()
        {
            var message = "Test message";
            var before = LogHubTests.app.GetTime();
            DateTime executed = DateTime.MinValue;
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(x => executed = x.Created);
            LogHubTests.app.GetLog().Debug(message);
            Assert.IsTrue(before <= executed, "Created date time should be greated than the time before creation.");
            Assert.IsTrue(executed <= LogHubTests.app.GetTime(), "Created date time should be lower than the time after creation.");
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceLogging()
        {
            var message = "Test message";
            var error = new ArgumentException("Test error");
            var called = 0;
            var repeat = 1000;
            Action<ILogEntry> handler = x => { called++; };
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(handler);
            for (int i = 0; i < repeat; i++)
            {
                LogHubTests.app.GetLog().Error(message, error);
                LogHubTests.app.GetLog().Debug(message);
                LogHubTests.app.GetLog().Info(message);
                LogHubTests.app.GetLog().Warning(message, error);
            }
            Assert.AreEqual(repeat * 4, called, "The event has not been called.");
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceTraceLog()
        {
            var message = "Test message";
            var error = new ArgumentException("Test error");
            var called = 0;
            var repeat = 150;
            Action<ILogEntry> handler = x => { called++; };
            LogHubTests.app.GetEventHub().Subscribe<ILogEntry>(handler);
            for (int i = 0; i < repeat; i++)
            {
                LogHubTests.app.GetLog().Trace(message);
            }
            Assert.AreEqual(repeat, called, "The event has not been called.");
        }
        #endregion

        #region Private fields and constants
        private static IApp app;
        #endregion
    }
}
