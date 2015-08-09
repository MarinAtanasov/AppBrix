// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Time.Configuration;
using System;
using System.Linq;

namespace AppBrix.Time
{
    /// <summary>
    /// A module which registers a default time service used for getting the current time.
    /// </summary>
    public class TimeModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            var dateTimeKind = this.App.GetConfig<TimeConfig>().Kind;
            this.timeService = this.CreateTimeService(dateTimeKind);
            this.App.GetResolver().Register(this);
            this.App.GetResolver().Register(this.timeService, this.timeService.GetType());
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private methods
        private ITimeService CreateTimeService(DateTimeKind kind)
        {
            switch (kind)
            {
                case DateTimeKind.Local:
                    return new LocalTimeService();
                case DateTimeKind.Utc:
                    return new UtcTimeService();
                case DateTimeKind.Unspecified:
                default:
                    throw new NotSupportedException("The specified DateTimeKind is not supported: " + kind);
            }
        }
        #endregion

        #region Private fields and constants
        private ITimeService timeService;
        #endregion
    }
}
