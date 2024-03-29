﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Contracts;
using System;

namespace AppBrix.Events.Schedule.Tests.Mocks;

internal sealed class ScheduledEventMock<T> : IScheduledEvent<T> where T : IEvent
{
    public ScheduledEventMock(T args, TimeSpan execute)
    {
        this.Event = args;
        this.execute = execute;
    }

    public ScheduledEventMock(T args, DateTime time)
    {
        this.Event = args;
        this.time = time;
    }

    public T Event { get; }

    public DateTime GetNextOccurrence(DateTime now)
    {
        this.time ??= now.Add(this.execute);
        return this.time.Value;
    }

    private readonly TimeSpan execute;
    private DateTime? time;
}
