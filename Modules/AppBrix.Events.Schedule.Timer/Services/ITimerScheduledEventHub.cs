// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Contracts;
using System;

namespace AppBrix.Events.Schedule.Timer.Services;

/// <summary>
/// Scheduler for timer based events.
/// </summary>
public interface ITimerScheduledEventHub
{
    /// <summary>
    /// Schedule an <see cref="IScheduledEvent{T}"/> to be executed once.
    /// </summary>
    /// <param name="args">The event to be executed.</param>
    /// <param name="dueTime">The amount of time to delay before the event should be raised.</param>
    /// <returns>The scheduled event, containing the original event.</returns>
    IScheduledEvent<T> Schedule<T>(T args, TimeSpan dueTime) where T : IEvent => this.Schedule(args, dueTime, TimeSpan.Zero);

    /// <summary>
    /// Schedule an <see cref="IScheduledEvent{T}"/> to be executed once.
    /// </summary>
    /// <param name="args">The event to be executed.</param>
    /// <param name="dueTime">The point in time after which the event should be raised.</param>
    /// <returns>The scheduled event, containing the original event.</returns>
    IScheduledEvent<T> Schedule<T>(T args, DateTime dueTime) where T : IEvent => this.Schedule(args, dueTime, TimeSpan.Zero);

    /// <summary>
    /// Schedule an <see cref="IScheduledEvent{T}"/> to be executed.
    /// </summary>
    /// <param name="args">The event to be executed.</param>
    /// <param name="dueTime">The amount of time to delay before the event should be raised.</param>
    /// <param name="period">The time interval between invocations of the event.</param>
    /// <returns>The scheduled event, containing the original event.</returns>
    IScheduledEvent<T> Schedule<T>(T args, TimeSpan dueTime, TimeSpan period) where T : IEvent;

    /// <summary>
    /// Schedule an <see cref="IScheduledEvent{T}"/> to be executed.
    /// </summary>
    /// <param name="args">The event to be executed.</param>
    /// <param name="dueTime">The point in time after which the event should be raised.</param>
    /// <param name="period">The time interval between invocations of the event.</param>
    /// <returns>The scheduled event, containing the original event.</returns>
    IScheduledEvent<T> Schedule<T>(T args, DateTime dueTime, TimeSpan period) where T : IEvent;

    /// <summary>
    /// Unschedules an <see cref="IScheduledEvent{T}"/> to stop it from executing.
    /// </summary>
    /// <param name="args">The scheduled event.</param>
    void Unschedule<T>(IScheduledEvent<T> args) where T : IEvent;
}
