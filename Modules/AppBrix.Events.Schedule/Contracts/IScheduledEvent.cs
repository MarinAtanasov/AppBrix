// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using System;

namespace AppBrix.Events.Schedule.Contracts;

/// <summary>
/// Scheduled event which decides when it needs to be called.
/// </summary>
public interface IScheduledEvent<out T> where T : IEvent
{
	/// <summary>
	/// Gets the event to be called at the next scheduled occurrence.
	/// </summary>
	T Event { get; }

	/// <summary>
	/// Calculates when the event needs to be called next.
	/// </summary>
	/// <param name="now">The base time to be used.</param>
	/// <returns>When the event needs to be called next.</returns>
	DateTime GetNextOccurrence(DateTime now);
}
