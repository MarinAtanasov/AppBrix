// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Logging.Entries;
using System;
using System.Linq;

namespace AppBrix.Logging.Configuration
{
    public sealed class LoggingConfig : IConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="LoggingConfig"/> with default property values.
        /// </summary>
        public LoggingConfig()
        {
            this.Async = true;
            this.LogLevel = LogLevel.All;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether to use an asychronous logger.
        /// Changing this value requires module/application restart.
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Gets or sets the minimal level in which the log entry events should be raised.
        /// </summary>
        public LogLevel LogLevel { get; set; }
        #endregion
    }
}
