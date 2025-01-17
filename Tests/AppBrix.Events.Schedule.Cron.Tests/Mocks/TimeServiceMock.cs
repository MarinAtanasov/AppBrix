// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Time.Services;
using System;

namespace AppBrix.Events.Schedule.Cron.Tests.Mocks;

internal sealed class TimeServiceMock : ITimeService
{
    #region Construction
    public TimeServiceMock(IApp app)
    {
        this.timeService = app.GetTimeService();
        this.SetTime(this.GetTime());
    }
    #endregion

    #region Public and overriden methods
    public DateTime GetTime() => this.dateTime ?? this.timeService.GetTime();

    public DateTimeOffset GetTimeLocal() => this.dateTimeOffset ?? this.timeService.GetTimeLocal();

    public DateTimeOffset GetTimeUtc() => this.dateTimeOffset ?? this.timeService.GetTimeUtc();

    public void SetOffset(DateTimeOffset time)
    {
        this.dateTime = new DateTime(time.DateTime.Ticks, time.Offset == TimeSpan.Zero ? DateTimeKind.Utc : DateTimeKind.Local);
        this.dateTimeOffset = time;
    }

    public void SetTime(DateTime time) => this.SetOffset(new DateTimeOffset(time));

    public DateTime ToDateTime(string time) => this.timeService.ToDateTime(time);

    public DateTimeOffset ToDateTimeOffset(string time) => this.timeService.ToDateTimeOffset(time);

    public string ToString(DateTime time) => this.timeService.ToString(time);

    public string ToString(DateTimeOffset time) => this.timeService.ToString(time);
    #endregion

    #region Private fields and constants
    private readonly ITimeService timeService;
    private DateTime? dateTime;
    private DateTimeOffset? dateTimeOffset;
    #endregion
}
