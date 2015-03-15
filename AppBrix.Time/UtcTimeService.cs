// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using AppBrix.Time.Configuration;
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
        #endregion
    }
}
