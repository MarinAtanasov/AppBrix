// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Contracts;
using AppBrix.Events.Schedule.Cron.Services;
using AppBrix.Lifecycle;
using System;

namespace AppBrix.Events.Schedule.Cron.Impl;

internal sealed class CronScheduledEventHub : ICronScheduledEventHub, IApplicationLifecycle
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

    #region ICronScheduledEventHub implementation
    public IScheduledEvent<T> Schedule<T>(T args, string expression) where T : IEvent
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentNullException(nameof(expression));

        var scheduled = new CronScheduledEvent<T>(args, Cronos.CronExpression.Parse(expression));
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
