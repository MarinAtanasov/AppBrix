// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Time;
using System;
using System.Linq;

namespace AppBrix.Caching.Memory.Tests.Mocks
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
        public DateTime GetTime()
        {
            return time ?? this.timeService.GetTime();
        }

        public void SetTime(DateTime time)
        {
            this.time = time;
        }

        public DateTime ToAppTime(DateTime time)
        {
            return this.timeService.ToAppTime(time);
        }

        public DateTime ToDateTime(string time)
        {
            return this.timeService.ToDateTime(time);
        }

        public string ToString(DateTime time)
        {
            return this.timeService.ToString(time);
        }
        #endregion

        #region Private fields and constants
        private readonly ITimeService timeService;
        private DateTime? time;
        #endregion
    }
}
