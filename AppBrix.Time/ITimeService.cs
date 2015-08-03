// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Time
{
    public interface ITimeService
    {
        /// <summary>
        /// Gets the current time.
        /// This should be used instead of DateTime.Now or DateTime.UtcNow.
        /// </summary>
        /// <returns></returns>
        DateTime GetTime();
        
        /// <summary>
        /// Converts the specified time to the configured application time kind.
        /// </summary>
        /// <param name="time">The specified time.</param>
        /// <returns>The converted time.</returns>
        DateTime ToAppTime(DateTime time);
    }
}
