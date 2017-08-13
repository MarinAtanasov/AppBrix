// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix.Events.Schedule.Config
{
    /// <summary>
    /// Configuration which sets how the scheduled events should behave.
    /// </summary>
    public sealed class ScheduledEventsConfig : IConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="ScheduledEventsConfig"/>.
        /// </summary>
        public ScheduledEventsConfig()
        {
            this.ExecutionCheck = TimeSpan.FromSeconds(1);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets how often the scheduled event hub should check events' execution time.
        /// </summary>
        public TimeSpan ExecutionCheck { get; set; }
        #endregion
    }
}
