// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration.Memory;
using AppBrix.Events;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Entries;
using AppBrix.Resolver;
using AppBrix.Tests;
using AppBrix.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AppBrix.Logging.Tests.Config
{
    [TestClass]
    public class LoggingConfigTests
    {
        #region Setup and cleanup
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            LoggingConfigTests.app = TestUtils.CreateTestApp(
                typeof(ResolverModule),
                typeof(MemoryConfigModule),
                typeof(EventsModule),
                typeof(TimeModule),
                typeof(LoggingModule));
            LoggingConfigTests.app.Start();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            LoggingConfigTests.app.Stop();
            LoggingConfigTests.app = null;
        }

        [TestInitialize]
        public void Initialize()
        {
            this.originalLogLevel = LoggingConfigTests.app.GetConfig<LoggingConfig>().LogLevel;
        }

        [TestCleanup]
        public void Cleanup()
        {
            LoggingConfigTests.app.GetConfig<LoggingConfig>().LogLevel = this.originalLogLevel;
            LoggingConfigTests.app.Reinitialize();
        }
        #endregion

        #region Tests
        [TestMethod]
        public void TestErrorLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            LoggingConfigTests.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Error;
            LoggingConfigTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                switch (x.Level)
                {
                    case LogLevel.Trace:
                        traceCalled = true;
                        break;
                    case LogLevel.Debug:
                        debugCalled = true;
                        break;
                    case LogLevel.Info:
                        infoCalled = true;
                        break;
                    case LogLevel.Warning:
                        warningCalled = true;
                        break;
                    case LogLevel.Error:
                        errorCalled = true;
                        break;
                    default:
                        throw new NotSupportedException(x.Level.ToString());
                }
            });
            LoggingConfigTests.app.GetLog().Error(message);
            LoggingConfigTests.app.GetLog().Warning(message);
            LoggingConfigTests.app.GetLog().Info(message);
            LoggingConfigTests.app.GetLog().Debug(message);
            LoggingConfigTests.app.GetLog().Trace(message);
            Assert.IsTrue(errorCalled, "The error event has not been called.");
            Assert.IsFalse(warningCalled, "The warning event has been called.");
            Assert.IsFalse(infoCalled, "The info event has been called.");
            Assert.IsFalse(debugCalled, "The debug event has been called.");
            Assert.IsFalse(traceCalled, "The trace event has been called.");
        }

        [TestMethod]
        public void TestWarningLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            LoggingConfigTests.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Warning;
            LoggingConfigTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                switch (x.Level)
                {
                    case LogLevel.Trace:
                        traceCalled = true;
                        break;
                    case LogLevel.Debug:
                        debugCalled = true;
                        break;
                    case LogLevel.Info:
                        infoCalled = true;
                        break;
                    case LogLevel.Warning:
                        warningCalled = true;
                        break;
                    case LogLevel.Error:
                        errorCalled = true;
                        break;
                    default:
                        throw new NotSupportedException(x.Level.ToString());
                }
            });
            LoggingConfigTests.app.GetLog().Error(message);
            LoggingConfigTests.app.GetLog().Warning(message);
            LoggingConfigTests.app.GetLog().Info(message);
            LoggingConfigTests.app.GetLog().Debug(message);
            LoggingConfigTests.app.GetLog().Trace(message);
            Assert.IsTrue(errorCalled, "The error event has not been called.");
            Assert.IsTrue(warningCalled, "The warning event has not been called.");
            Assert.IsFalse(infoCalled, "The info event has been called.");
            Assert.IsFalse(debugCalled, "The debug event has been called.");
            Assert.IsFalse(traceCalled, "The trace event has been called.");
        }

        [TestMethod]
        public void TestInfoLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            LoggingConfigTests.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Info;
            LoggingConfigTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                switch (x.Level)
                {
                    case LogLevel.Trace:
                        traceCalled = true;
                        break;
                    case LogLevel.Debug:
                        debugCalled = true;
                        break;
                    case LogLevel.Info:
                        infoCalled = true;
                        break;
                    case LogLevel.Warning:
                        warningCalled = true;
                        break;
                    case LogLevel.Error:
                        errorCalled = true;
                        break;
                    default:
                        throw new NotSupportedException(x.Level.ToString());
                }
            });
            LoggingConfigTests.app.GetLog().Error(message);
            LoggingConfigTests.app.GetLog().Warning(message);
            LoggingConfigTests.app.GetLog().Info(message);
            LoggingConfigTests.app.GetLog().Debug(message);
            LoggingConfigTests.app.GetLog().Trace(message);
            Assert.IsTrue(errorCalled, "The error event has not been called.");
            Assert.IsTrue(warningCalled, "The warning event has not been called.");
            Assert.IsTrue(infoCalled, "The info event has not been called.");
            Assert.IsFalse(debugCalled, "The debug event has been called.");
            Assert.IsFalse(traceCalled, "The trace event has been called.");
        }

        [TestMethod]
        public void TestDebugLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            LoggingConfigTests.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Debug;
            LoggingConfigTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                switch (x.Level)
                {
                    case LogLevel.Trace:
                        traceCalled = true;
                        break;
                    case LogLevel.Debug:
                        debugCalled = true;
                        break;
                    case LogLevel.Info:
                        infoCalled = true;
                        break;
                    case LogLevel.Warning:
                        warningCalled = true;
                        break;
                    case LogLevel.Error:
                        errorCalled = true;
                        break;
                    default:
                        throw new NotSupportedException(x.Level.ToString());
                }
            });
            LoggingConfigTests.app.GetLog().Error(message);
            LoggingConfigTests.app.GetLog().Warning(message);
            LoggingConfigTests.app.GetLog().Info(message);
            LoggingConfigTests.app.GetLog().Debug(message);
            LoggingConfigTests.app.GetLog().Trace(message);
            Assert.IsTrue(errorCalled, "The error event has not been called.");
            Assert.IsTrue(warningCalled, "The warning event has not been called.");
            Assert.IsTrue(infoCalled, "The info event has not been called.");
            Assert.IsTrue(debugCalled, "The debug event has not been called.");
            Assert.IsFalse(traceCalled, "The trace event has been called.");
        }

        [TestMethod]
        public void TestTraceLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            LoggingConfigTests.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Trace;
            LoggingConfigTests.app.GetEventHub().Subscribe<ILogEntry>(x =>
            {
                switch (x.Level)
                {
                    case LogLevel.Trace:
                        traceCalled = true;
                        break;
                    case LogLevel.Debug:
                        debugCalled = true;
                        break;
                    case LogLevel.Info:
                        infoCalled = true;
                        break;
                    case LogLevel.Warning:
                        warningCalled = true;
                        break;
                    case LogLevel.Error:
                        errorCalled = true;
                        break;
                    default:
                        throw new NotSupportedException(x.Level.ToString());
                }
            });
            LoggingConfigTests.app.GetLog().Error(message);
            LoggingConfigTests.app.GetLog().Warning(message);
            LoggingConfigTests.app.GetLog().Info(message);
            LoggingConfigTests.app.GetLog().Debug(message);
            LoggingConfigTests.app.GetLog().Trace(message);
            Assert.IsTrue(errorCalled, "The error event has not been called.");
            Assert.IsTrue(warningCalled, "The warning event has not been called.");
            Assert.IsTrue(infoCalled, "The info event has not been called.");
            Assert.IsTrue(debugCalled, "The debug event has not been called.");
            Assert.IsTrue(traceCalled, "The trace event has not been called.");
        }
        #endregion

        #region Private fields and constants
        private static IApp app;
        private LogLevel originalLogLevel;
        #endregion
    }
}
