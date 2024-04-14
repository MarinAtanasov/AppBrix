// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Contracts;
using AppBrix.Events.Schedule.Timer.Services;
using AppBrix.Lifecycle;
using System;

namespace AppBrix.Events.Schedule.Timer.Impl;

internal sealed class TimerScheduledEventHub : ITimerScheduledEventHub, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
    }

    public void Uninitialize()
    {
        this.app = null!;
    }
    #endregion

    #region ITimerScheduledEventHub implementation
    public IScheduledEvent<T> Schedule<T>(T args, TimeSpan dueTime, TimeSpan period) where T : IEvent
    {
        if (dueTime < TimeSpan.Zero)
            throw new ArgumentException($"Negative {nameof(dueTime)}: {dueTime}");

        return this.Schedule(args, this.app.GetTime().Add(dueTime), period);
    }

    public IScheduledEvent<T> Schedule<T>(T args, DateTime dueTime, TimeSpan period) where T : IEvent
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));
        if (period < TimeSpan.Zero)
            throw new ArgumentException($"Negative {nameof(period)}: {period}");

        var scheduled = new TimerScheduledEvent<T>(args, dueTime, period);
        this.app.GetScheduledEventHub().Schedule(scheduled);
        return scheduled;
    }

    public void Unschedule<T>(IScheduledEvent<T> args) where T : IEvent
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        this.app.GetScheduledEventHub().Unschedule(args);
    }
    #endregion

    #region Private fields and constants
    private IApp app = null!;
    #endregion
}
