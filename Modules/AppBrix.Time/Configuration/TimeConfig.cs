// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using System;

namespace AppBrix.Time.Configuration;

/// <summary>
/// Configures the <see cref="DateTimeKind"/> and <see cref="DateTime"/> format to be used within the application.
/// </summary>
public sealed class TimeConfig : IConfig
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="TimeConfig"/> with default values for the properties.
    /// </summary>
    public TimeConfig()
    {
        this.Kind = DateTimeKind.Utc;
        this.Format = @"yyyy-MM-ddTHH:mm:ss.fffK";
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the time kind to be used inside the application.
    /// Supported values: Utc, Local
    /// Changing this property requires module/application reinitialization.
    /// </summary>
    public DateTimeKind Kind { get; set; }

    /// <summary>
    /// Gets or sets the time string format to be used inside the application.
    /// Changing this property requires module/application reinitialization.
    /// </summary>
    public string Format { get; set; }
    #endregion
}
