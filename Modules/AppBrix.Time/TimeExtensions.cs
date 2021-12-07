// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Time;
using System;
using AppBrix.Configuration;
using AppBrix.Time.Configuration;
using AppBrix.Time.Services;

namespace AppBrix;

/// <summary>
/// Extension methods for the <see cref="TimeModule"/>.
/// </summary>
public static class TimeExtensions
{
    /// <summary>
    /// Gets the application's currently registered <see cref="ITimeService"/>
    /// </summary>
    /// <param name="app">The application.</param>
    /// <returns>The registered time service.</returns>
    public static ITimeService GetTimeService(this IApp app) => (ITimeService)app.Get(typeof(ITimeService));

    /// <summary>
    /// Gets the <see cref="TimeConfig"/> from <see cref="IConfigService"/>.
    /// </summary>
    /// <param name="service">The configuration service.</param>
    /// <returns>The <see cref="TimeConfig"/>.</returns>
    public static TimeConfig GetTimeConfig(this IConfigService service) => (TimeConfig)service.Get(typeof(TimeConfig));

    /// <summary>
    /// A shorthand for getting the current time
    /// from the registered <see cref="ITimeService"/>.
    /// </summary>
    /// <param name="app">The currently running application.</param>
    /// <returns>The current <see cref="DateTime"/>.</returns>
    public static DateTime GetTime(this IApp app) => app.GetTimeService().GetTime();
}
