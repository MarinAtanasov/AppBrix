// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Contracts;
using System;

namespace AppBrix.Events.Schedule.Cron.Impl;

internal sealed class CronScheduledEvent<T> : IScheduledEvent<T> where T : IEvent
{
    #region Construction
    public CronScheduledEvent(T args, Cronos.CronExpression expression)
    {
        this.Event = args;
        this.expression = expression;
    }
    #endregion

    #region Properties
    public T Event { get; }
    #endregion

    #region Public and overriden methods
    public DateTime GetNextOccurrence(DateTime now) => this.expression.GetNextOccurrence(now) ?? now;
    #endregion

    #region Private fields and constants
    private readonly Cronos.CronExpression expression;
    #endregion
}
