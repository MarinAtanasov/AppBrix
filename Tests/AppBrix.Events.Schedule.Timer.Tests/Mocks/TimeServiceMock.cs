// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Time;
using System;

namespace AppBrix.Events.Schedule.Timer.Tests.Mocks
{
    internal sealed class TimeServiceMock : ITimeService
    {
        #region Construction
        public TimeServiceMock(IApp app)
        {
            this.timeService = app.GetTimeService();
        }
        #endregion

        #region Public and overriden methods
        public DateTime GetTime() => dateTime ?? this.timeService.GetTime();

        public void SetTime(DateTime time) => this.dateTime = time;

        public DateTime ToAppTime(DateTime time) => this.timeService.ToAppTime(time);

        public DateTime ToDateTime(string time) => this.timeService.ToDateTime(time);

        public string ToString(DateTime time) => this.timeService.ToString(time);
        #endregion

        #region Private fields and constants
        private readonly ITimeService timeService;
        private DateTime? dateTime;
        #endregion
    }
}
