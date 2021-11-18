// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Globalization;

namespace AppBrix.Time.Impl;

internal abstract class TimeServiceBase : ITimeService
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="TimeServiceBase"/>.
    /// </summary>
    /// <param name="format">The string format to be used when converting a <see cref="DateTime"/> to a <see cref="string"/>.</param>
    protected TimeServiceBase(string format)
    {
        this.format = format;
    }
    #endregion

    #region ITimeService implementation
    public abstract DateTime GetTime();

    public abstract DateTime ToAppTime(DateTime time);

    public DateTime ToDateTime(string time) => DateTime.ParseExact(time, this.format, CultureInfo.InvariantCulture);

    public string ToString(DateTime time) => time.ToString(this.format);
    #endregion

    #region Private fields and constants
    private readonly string format;
    #endregion
}
