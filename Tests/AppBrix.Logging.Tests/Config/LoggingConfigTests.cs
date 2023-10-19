// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Contracts;
using AppBrix.Logging.Events;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Logging.Tests.Config;

public sealed class LoggingConfigTests : TestsBase<LoggingModule>
{
    #region Setup and cleanup
    public LoggingConfigTests() => this.App.Start();
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
        this.App.ConfigService.GetLoggingConfig().LogLevel = LogLevel.Error;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
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
        this.App.GetLogHub().Error(message);
        this.App.GetLogHub().Warning(message);
        this.App.GetLogHub().Info(message);
        this.App.GetLogHub().Debug(message);
        this.App.GetLogHub().Trace(message);
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
        this.App.ConfigService.GetLoggingConfig().LogLevel = LogLevel.Warning;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
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
        this.App.GetLogHub().Error(message);
        this.App.GetLogHub().Warning(message);
        this.App.GetLogHub().Info(message);
        this.App.GetLogHub().Debug(message);
        this.App.GetLogHub().Trace(message);
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
        this.App.ConfigService.GetLoggingConfig().LogLevel = LogLevel.Info;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
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
        this.App.GetLogHub().Error(message);
        this.App.GetLogHub().Warning(message);
        this.App.GetLogHub().Info(message);
        this.App.GetLogHub().Debug(message);
        this.App.GetLogHub().Trace(message);
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
        this.App.ConfigService.GetLoggingConfig().LogLevel = LogLevel.Debug;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
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
        this.App.GetLogHub().Error(message);
        this.App.GetLogHub().Warning(message);
        this.App.GetLogHub().Info(message);
        this.App.GetLogHub().Debug(message);
        this.App.GetLogHub().Trace(message);
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
        this.App.ConfigService.GetLoggingConfig().LogLevel = LogLevel.Trace;
        this.App.GetEventHub().Subscribe<ILogEntry>(x =>
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
        this.App.GetLogHub().Error(message);
        this.App.GetLogHub().Warning(message);
        this.App.GetLogHub().Info(message);
        this.App.GetLogHub().Debug(message);
        this.App.GetLogHub().Trace(message);
        errorCalled.Should().BeTrue("the error event should have been called");
        warningCalled.Should().BeTrue("the warning event should have been called");
        infoCalled.Should().BeTrue("the info event should have been called");
        debugCalled.Should().BeTrue("the debug event should have been called");
        traceCalled.Should().BeTrue("the trace event should have been called");
    }
    #endregion
}
