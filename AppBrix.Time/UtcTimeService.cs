// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Time
{
    internal sealed class UtcTimeService : TimeServiceBase
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="UtcTimeService"/>.
        /// </summary>
        /// <param name="format">The string format to be used when converting a <see cref="DateTime"/> to a <see cref="string"/>.</param>
        public UtcTimeService(string format) : base(format)
        {
        }
        #endregion

        #region ITimeService implementation
        public override DateTime GetTime()
        {
            return DateTime.UtcNow;
        }

        public override DateTime ToAppTime(DateTime time)
        {
            return time.Kind == DateTimeKind.Utc ? time : time.ToUniversalTime();
        }
        #endregion
    }
}
