// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Contracts;
using System;

namespace AppBrix.Events.Schedule.Impl;

internal abstract class PriorityQueueItem
{
    #region Properties
    public abstract object ScheduledEvent { get; }

    public DateTime Occurrence { get; protected set; }
    #endregion

    #region Public and overriden methods
    public abstract void Execute();

    public abstract void MoveToNextOccurrence(DateTime now);
    #endregion
}

internal sealed class PriorityQueueItem<T> : PriorityQueueItem where T : IEvent
{
    #region Construction
    public PriorityQueueItem(IApp app, IScheduledEvent<T> scheduledEvent)
    {
        this.app = app;
        this.scheduledEvent = scheduledEvent;
    }
    #endregion

    #region Properties
    public override object ScheduledEvent => this.scheduledEvent;
    #endregion

    #region Public and overriden methods
    public override void Execute()
    {
        try
        {
            this.app.GetEventHub().Raise(this.scheduledEvent.Event);
        }
        catch (Exception)
        {
        }
    }

    public override void MoveToNextOccurrence(DateTime now) => this.Occurrence = this.scheduledEvent.GetNextOccurrence(now);
    #endregion

    #region Private fields and constants
    private readonly IApp app;
    private readonly IScheduledEvent<T> scheduledEvent;
    #endregion
}
