// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Events;
using AppBrix.Factory;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Entries;
using AppBrix.Tests;
using AppBrix.Time;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Logging.Tests.Config
{
    public sealed class LoggingConfigTests
    {
        #region Setup and cleanup
        public LoggingConfigTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ContainerModule),
                typeof(EventsModule),
                typeof(FactoryModule),
                typeof(LoggingModule),
                typeof(TimeModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestErrorLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            this.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Error;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
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
            this.app.GetLog().Error(message);
            this.app.GetLog().Warning(message);
            this.app.GetLog().Info(message);
            this.app.GetLog().Debug(message);
            this.app.GetLog().Trace(message);
            errorCalled.Should().BeTrue("the error event should have been called");
            warningCalled.Should().BeFalse("the warning event should not have been called");
            infoCalled.Should().BeFalse("the info event should not have been called");
            debugCalled.Should().BeFalse("the debug event should not have been called");
            traceCalled.Should().BeFalse("the trace event should not have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestWarningLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            this.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Warning;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
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
            this.app.GetLog().Error(message);
            this.app.GetLog().Warning(message);
            this.app.GetLog().Info(message);
            this.app.GetLog().Debug(message);
            this.app.GetLog().Trace(message);
            errorCalled.Should().BeTrue("the error event should have been called");
            warningCalled.Should().BeTrue("the warning event should have been called");
            infoCalled.Should().BeFalse("the info event should not have been called");
            debugCalled.Should().BeFalse("the debug event should not have been called");
            traceCalled.Should().BeFalse("the trace event should not have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInfoLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            this.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Info;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
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
            this.app.GetLog().Error(message);
            this.app.GetLog().Warning(message);
            this.app.GetLog().Info(message);
            this.app.GetLog().Debug(message);
            this.app.GetLog().Trace(message);
            errorCalled.Should().BeTrue("the error event should have been called");
            warningCalled.Should().BeTrue("the warning event should have been called");
            infoCalled.Should().BeTrue("the info event should have been called");
            debugCalled.Should().BeFalse("the debug event should not have been called");
            traceCalled.Should().BeFalse("the trace event should not have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestDebugLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            this.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Debug;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
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
            this.app.GetLog().Error(message);
            this.app.GetLog().Warning(message);
            this.app.GetLog().Info(message);
            this.app.GetLog().Debug(message);
            this.app.GetLog().Trace(message);
            errorCalled.Should().BeTrue("the error event should have been called");
            warningCalled.Should().BeTrue("the warning event should have been called");
            infoCalled.Should().BeTrue("the info event should have been called");
            debugCalled.Should().BeTrue("the debug event should have been called");
            traceCalled.Should().BeFalse("the trace event should not have been called");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestTraceLevelConfig()
        {
            var message = "Test message";
            var errorCalled = false;
            var warningCalled = false;
            var infoCalled = false;
            var debugCalled = false;
            var traceCalled = false;
            this.app.GetConfig<LoggingConfig>().LogLevel = LogLevel.Trace;
            this.app.GetEventHub().Subscribe<ILogEntry>(x =>
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
            this.app.GetLog().Error(message);
            this.app.GetLog().Warning(message);
            this.app.GetLog().Info(message);
            this.app.GetLog().Debug(message);
            this.app.GetLog().Trace(message);
            errorCalled.Should().BeTrue("the error event should have been called");
            warningCalled.Should().BeTrue("the warning event should have been called");
            infoCalled.Should().BeTrue("the info event should have been called");
            debugCalled.Should().BeTrue("the debug event should have been called");
            traceCalled.Should().BeTrue("the trace event should have been called");
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
