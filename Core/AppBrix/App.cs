// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Modules;

namespace AppBrix;

/// <summary>
/// Static class used for loading of a default app.
/// </summary>
public static class App
{
    #region Public and overriden methods
    /// <summary>
    /// Creates a default application with a specified configuration service.
    /// </summary>
    /// <param name="configService">The configuration service.</param>
    /// <returns>The created app.</returns>
    public static IApp Create(IConfigService configService) => new DefaultApp(configService);

    /// <summary>
    /// Creates and starts a default application with a specified configuration service.
    /// </summary>
    /// <param name="configService">The configuration service.</param>
    /// <returns>The created and started app.</returns>
    public static IApp Start(IConfigService configService)
    {
        var app = App.Create(configService);
        app.Start();
        return app;
    }

    /// <summary>
    /// Creates a default application with a specified configuration service.
    /// Registers the provided module type in the <see cref="AppConfig"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="MainModuleBase"/> to be registered.</typeparam>
    /// <param name="configService">The configuration service.</param>
    /// <returns>The created app.</returns>
    public static IApp Create<T>(IConfigService configService) where T : MainModuleBase, new()
    {
        var mainModuleConfigElement = ModuleConfigElement.Create<T>();
        var type = mainModuleConfigElement.Type;

        var modules = configService.GetAppConfig().Modules;
        foreach (var module in modules)
        {
            if (module.Type == type)
                return App.Create(configService);
        }
        modules.Add(mainModuleConfigElement);

        return App.Create(configService);
    }

    /// <summary>
    /// Creates and starts a default application with a specified configuration service.
    /// Registers the provided module type in the <see cref="AppConfig"/> before starting.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="MainModuleBase"/> to be registered.</typeparam>
    /// <param name="configService">The configuration service.</param>
    /// <returns>The created and started app.</returns>
    public static IApp Start<T>(IConfigService configService) where T : MainModuleBase, new()
    {
        var app = App.Create<T>(configService);
        app.Start();
        return app;
    }
    #endregion
}
