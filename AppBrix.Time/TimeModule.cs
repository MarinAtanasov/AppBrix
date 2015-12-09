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
            var config = this.App.GetConfig<TimeConfig>();
            var dateTimeKind = config.Kind;
            var format = config.Format;
            var timeService = this.CreateTimeService(dateTimeKind, format);
            this.App.GetResolver().Register(this);
            this.App.GetResolver().Register(timeService, timeService.GetType());
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private methods
        private ITimeService CreateTimeService(DateTimeKind kind, string format)
        {
            switch (kind)
            {
                case DateTimeKind.Local:
                    return new LocalTimeService(format);
                case DateTimeKind.Utc:
                    return new UtcTimeService(format);
                case DateTimeKind.Unspecified:
                default:
                    throw new NotSupportedException("The specified DateTimeKind is not supported: " + kind);
            }
        }
        #endregion
    }
}
