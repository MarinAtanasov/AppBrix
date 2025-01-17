// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Contracts;
using AppBrix.Logging.Events;
using AppBrix.Testing;
using System;

namespace AppBrix.Logging.Tests.Config;

[TestClass]
public sealed class LoggingConfigTests : TestsBase<LoggingModule>
{
    #region Test lifecycle
    protected override void Initialize() => this.App.Start();
    #endregion

    #region Tests
    [Test, Functional]
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
        this.Assert(errorCalled, "the error event should have been called");
        this.Assert(warningCalled == false, "the warning event should not have been called");
        this.Assert(infoCalled == false, "the info event should not have been called");
        this.Assert(debugCalled == false, "the debug event should not have been called");
        this.Assert(traceCalled == false, "the trace event should not have been called");
    }

    [Test, Functional]
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
        this.Assert(errorCalled, "the error event should have been called");
        this.Assert(warningCalled, "the warning event should have been called");
        this.Assert(infoCalled == false, "the info event should not have been called");
        this.Assert(debugCalled == false, "the debug event should not have been called");
        this.Assert(traceCalled == false, "the trace event should not have been called");
    }

    [Test, Functional]
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
        this.Assert(errorCalled, "the error event should have been called");
        this.Assert(warningCalled, "the warning event should have been called");
        this.Assert(infoCalled, "the info event should have been called");
        this.Assert(debugCalled == false, "the debug event should not have been called");
        this.Assert(traceCalled == false, "the trace event should not have been called");
    }

    [Test, Functional]
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
        this.Assert(errorCalled, "the error event should have been called");
        this.Assert(warningCalled, "the warning event should have been called");
        this.Assert(infoCalled, "the info event should have been called");
        this.Assert(debugCalled, "the debug event should have been called");
        this.Assert(traceCalled == false, "the trace event should not have been called");
    }

    [Test, Functional]
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
        this.Assert(errorCalled, "the error event should have been called");
        this.Assert(warningCalled, "the warning event should have been called");
        this.Assert(infoCalled, "the info event should have been called");
        this.Assert(debugCalled, "the debug event should have been called");
        this.Assert(traceCalled, "the trace event should have been called");
    }
    #endregion
}
