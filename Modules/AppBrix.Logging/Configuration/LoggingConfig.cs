// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Logging.Contracts;

namespace AppBrix.Logging.Configuration;

/// <summary>
/// Configures the logging for the application.
/// </summary>
public sealed class LoggingConfig : IConfig
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="LoggingConfig"/> with default property values.
    /// </summary>
    public LoggingConfig()
    {
        this.Async = false;
        this.Level = LogLevel.Trace;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets whether to use an asynchronous logger.
    /// Changing this value requires module/application restart.
    /// </summary>
    public bool Async { get; set; }

    /// <summary>
    /// Gets or sets the minimal level in which the log entry events should be raised.
    /// </summary>
    public LogLevel Level { get; set; }
    #endregion
}
