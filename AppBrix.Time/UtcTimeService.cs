// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Time
{
    internal sealed class UtcTimeService : ITimeService
    {
        #region ITimeService implementation
        public DateTime GetTime()
        {
            return DateTime.UtcNow;
        }

        public DateTime ToAppTime(DateTime time)
        {
            return time.Kind == DateTimeKind.Utc ? time : time.ToUniversalTime();
        }
        #endregion
    }
}
