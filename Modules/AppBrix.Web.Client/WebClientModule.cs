// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Time;
using AppBrix.Web.Client.Impl;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppBrix.Web.Client;

/// <summary>
/// A module for http communication.
/// </summary>
public sealed class WebClientModule : ModuleBase
{
    #region Properties
    /// <summary>
    /// Gets the types of the modules which are direct dependencies for the current module.
    /// This is used to determine the order in which the modules are loaded.
    /// </summary>
    public override IEnumerable<Type> Dependencies => new[] { typeof(FactoryModule), typeof(TimeModule) };
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Initializes the module.
    /// Automatically called by <see cref="ModuleBase.Initialize"/>
    /// </summary>
    /// <param name="context">The initialization context.</param>
    protected override void Initialize(IInitializeContext context)
    {
        this.App.Container.Register(this);

        var config = this.App.ConfigService.GetWebClientConfig();
        this.client = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = config.MaxConnectionsPerServer }, true)
        {
            Timeout = config.RequestTimeout
        };
        this.App.Container.Register(this.client);

        this.httpClientFactory.Initialize(context);
        this.App.Container.Register(this.httpClientFactory);
        this.App.GetFactoryService().Register(this.CreateRequest);

        this.jsonSerializerOptions = new JsonSerializerOptions();
        this.jsonSerializerOptions.Converters.Add(new JsonStringDateTimeConverter(this.App.GetTimeService()));
        this.jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        this.jsonSerializerOptions.Converters.Add(new JsonStringTimeSpanConverter());
        this.jsonSerializerOptions.Converters.Add(new JsonStringVersionConverter());
        this.App.Container.Register(this.jsonSerializerOptions);
    }

    /// <summary>
    /// Uninitializes the module.
    /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
    /// </summary>
    protected override void Uninitialize()
    {
        this.jsonSerializerOptions = null;
        this.httpClientFactory.Uninitialize();
        this.client?.Dispose();
        this.client = null;
    }
    #endregion

    #region Private methods
    private HttpRequest CreateRequest() => new HttpRequest(this.App);
    #endregion

    #region Private fields and constants
    private readonly HttpClientFactory httpClientFactory = new HttpClientFactory();
    private HttpClient? client;
    private JsonSerializerOptions? jsonSerializerOptions;
    #endregion
}
