// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Schedule.Timer.Services;

namespace AppBrix;

/// <summary>
/// Extension methods for easier manipulation of AppBrix timer scheduled events.
/// </summary>
public static class TimerScheduledEventsExtensions
{
	/// <summary>
	/// Gets the currently loaded cron scheduled event hub.
	/// </summary>
	/// <param name="app">The current application.</param>
	/// <returns>The event hub.</returns>
	public static ITimerScheduledEventHub GetTimerScheduledEventHub(this IApp app) => (ITimerScheduledEventHub)app.Get(typeof(ITimerScheduledEventHub));
}
