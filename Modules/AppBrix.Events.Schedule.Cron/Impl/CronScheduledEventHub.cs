// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Events.Schedule.Cron.Impl
{
    internal sealed class CronScheduledEventHub : ICronScheduledEventHub, IApplicationLifecycle
    {
        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.app = null;
        }

        public IScheduledEvent<T> Schedule<T>(T args, string expresssion) where T : IEvent
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (string.IsNullOrEmpty(expresssion))
                throw new ArgumentNullException(nameof(expresssion));

            var scheduled = new CronScheduledEvent<T>(args, NCrontab.CrontabSchedule.Parse(expresssion));
            this.app.GetScheduledEventHub().Schedule(scheduled);
            return scheduled;
        }

        public void Unschedule<T>(IScheduledEvent<T> args) where T : IEvent
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            this.app.GetScheduledEventHub().Unschedule(args);
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
