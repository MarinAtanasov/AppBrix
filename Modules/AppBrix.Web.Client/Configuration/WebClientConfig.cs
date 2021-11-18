// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using System;

namespace AppBrix.Web.Client.Configuration;

/// <summary>
/// Configures the web client.
/// </summary>
public sealed class WebClientConfig : IConfig
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="WebClientConfig"/> with default values for the properties.
    /// </summary>
    public WebClientConfig()
    {
        this.MaxConnectionsPerServer = 128;
        this.RequestTimeout = TimeSpan.FromMinutes(5);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the maximum connections per server.
    /// </summary>
    public int MaxConnectionsPerServer { get; set; }

    /// <summary>
    /// Gets or sets the timeout used when making HTTP requests.
    /// </summary>
    public TimeSpan RequestTimeout { get; set; }
    #endregion
}
