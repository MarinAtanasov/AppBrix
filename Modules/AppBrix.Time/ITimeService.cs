// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Time
{
    /// <summary>
    /// Service which operates with <see cref="DateTime"/>.
    /// </summary>
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

        /// <summary>
        /// Converts a given <see cref="DateTime"/> to a predefined <see cref="string"/> representation.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The string representation of the time.</returns>
        string ToString(DateTime time);

        /// <summary>
        /// Converts a given <see cref="string"/> to a <see cref="DateTime"/> in a system time kind.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        DateTime ToDateTime(string time);
    }
}
