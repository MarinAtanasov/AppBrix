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
    public sealed class TimeModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            var config = this.App.GetConfig<TimeConfig>();
            var timeService = this.CreateTimeService(config.Kind, config.Format);
            this.App.GetContainer().Register(timeService, timeService.GetType());
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
                default:
                    throw new NotSupportedException(string.Concat("The specified " + kind.GetType().Name +  " is not supported: ", kind));
            }
        }
        #endregion
    }
}
