// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
