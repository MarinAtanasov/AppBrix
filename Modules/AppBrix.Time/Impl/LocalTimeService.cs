// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Time.Impl
{
    internal sealed class LocalTimeService : TimeServiceBase
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="LocalTimeService"/>.
        /// </summary>
        /// <param name="format">The string format to be used when converting a <see cref="DateTime"/> to a <see cref="string"/>.</param>
        public LocalTimeService(string format) : base(format)
        {
        }
        #endregion

        #region ITimeService implementation
        public override DateTime GetTime() => DateTime.Now;

        public override DateTime ToAppTime(DateTime time) => time.ToLocalTime();
        #endregion
    }
}
