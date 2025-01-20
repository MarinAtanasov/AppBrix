// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory;
using AppBrix.Lifecycle;
using AppBrix.Logging;
using AppBrix.Modules;
using AppBrix.Web.Server.Events;
using AppBrix.Web.Server.Impl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace AppBrix.Web.Server;

/// <summary>
/// Modules used for working with Mvc or Web Api controllers.
/// For dependency injection of the current app inside the controllers' constructors,
/// use the <see cref="T:IServiceCollection.AddApp"/> extension method inside the
/// <see cref="M:ConfigureServices"/> method.
/// </summary>
public sealed class WebServerModule : ModuleBase
{
    #region Properties
    /// <summary>
    /// Gets the types of the modules which are direct dependencies for the current module.
    /// This is used to determine the order in which the modules are loaded.
    /// </summary>
    public override IEnumerable<Type> Dependencies => [typeof(FactoryModule), typeof(LoggingModule)];
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

        this.App.GetEventHub().Subscribe<IConfigureWebAppBuilder>(this.ConfigureWebAppBuilder);
        this.App.GetEventHub().Subscribe<IConfigureWebApp>(this.ConfigureWebApp);
    }

    /// <summary>
    /// Uninitializes the module.
    /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
    /// </summary>
    protected override void Uninitialize()
    {
        this.App.GetEventHub().Unsubscribe<IConfigureWebAppBuilder>(this.ConfigureWebAppBuilder);
        this.App.GetEventHub().Unsubscribe<IConfigureWebApp>(this.ConfigureWebApp);
    }
    #endregion

    #region Private methods
    private void AddJsonOptions(JsonOptions options)
    {
        var appOptions = this.App.Get<JsonSerializerOptions>();
        var appConverters = appOptions.Converters;
        var hostConverters = options.JsonSerializerOptions.Converters;
        foreach (var converter in appConverters)
        {
            hostConverters.Add(converter);
        }

        options.JsonSerializerOptions.AllowTrailingCommas = appOptions.AllowTrailingCommas;
        options.JsonSerializerOptions.DefaultBufferSize = appOptions.DefaultBufferSize;
        options.JsonSerializerOptions.DefaultIgnoreCondition = appOptions.DefaultIgnoreCondition;
        options.JsonSerializerOptions.DictionaryKeyPolicy = appOptions.DictionaryKeyPolicy;
        options.JsonSerializerOptions.Encoder = appOptions.Encoder;
        options.JsonSerializerOptions.IgnoreReadOnlyFields = appOptions.IgnoreReadOnlyFields;
        options.JsonSerializerOptions.IgnoreReadOnlyProperties = appOptions.IgnoreReadOnlyProperties;
        options.JsonSerializerOptions.IncludeFields = appOptions.IncludeFields;
        options.JsonSerializerOptions.MaxDepth = appOptions.MaxDepth;
        options.JsonSerializerOptions.NumberHandling = appOptions.NumberHandling;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = appOptions.PropertyNameCaseInsensitive;
        options.JsonSerializerOptions.PropertyNamingPolicy = appOptions.PropertyNamingPolicy;
        options.JsonSerializerOptions.ReadCommentHandling = appOptions.ReadCommentHandling;
        options.JsonSerializerOptions.ReferenceHandler = appOptions.ReferenceHandler;
        options.JsonSerializerOptions.UnknownTypeHandling = appOptions.UnknownTypeHandling;
        options.JsonSerializerOptions.WriteIndented = appOptions.WriteIndented;
    }

    private void ApplicationStarted() => this.App.GetEventHub().Raise(new HostApplicationStarted());

    private void ApplicationStopping() => this.App.GetEventHub().Raise(new HostApplicationStopping());

    private void ApplicationStopped()
    {
        this.App.GetEventHub().Raise(new HostApplicationStopped());
        this.App.Stop();
    }

    private void ConfigureWebAppBuilder(IConfigureWebAppBuilder args)
    {
        var level = this.App.ConfigService.GetLoggingConfig().Level switch
        {
            AppBrix.Logging.Contracts.LogLevel.Critical => LogLevel.Critical,
            AppBrix.Logging.Contracts.LogLevel.Debug => LogLevel.Debug,
            AppBrix.Logging.Contracts.LogLevel.Error => LogLevel.Error,
            AppBrix.Logging.Contracts.LogLevel.Info => LogLevel.Information,
            AppBrix.Logging.Contracts.LogLevel.None => LogLevel.None,
            AppBrix.Logging.Contracts.LogLevel.Trace => LogLevel.Trace,
            AppBrix.Logging.Contracts.LogLevel.Warning => LogLevel.Warning,
            _ => LogLevel.Trace
        };

        args.Builder.Logging
            .ClearProviders()
            .AddProvider(this.App.Get<ILoggerProvider>())
            .SetMinimumLevel(level)
            .AddFilter(x => x >= level);

        args.Builder.Services
            .AddSingleton(this.App)
            .AddControllers()
            .AddJsonOptions(this.AddJsonOptions);
    }

    private void ConfigureWebApp(IConfigureWebApp args)
    {
        var applicationLifetime = args.App.Services.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStarted.Register(this.ApplicationStarted);
        applicationLifetime.ApplicationStopping.Register(this.ApplicationStopping);
        applicationLifetime.ApplicationStopped.Register(this.ApplicationStopped);
        var httpClientFactory = args.App.Services.GetService<IHttpClientFactory>();
        if (httpClientFactory is not null)
            this.App.Container.Register(httpClientFactory);
    }
    #endregion
}
