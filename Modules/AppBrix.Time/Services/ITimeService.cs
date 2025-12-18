// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Time.Services;

/// <summary>
/// Service which operates with <see cref="DateTime"/>.
/// </summary>
public interface ITimeService
{
	/// <summary>
	/// Gets the current UTC date and time.
	/// This should be used instead of <see cref="DateTime.Now"/> or <see cref="DateTime.UtcNow"/>.
	/// </summary>
	/// <returns>The current date and time.</returns>
	DateTime GetTime();

	/// <summary>
	/// Gets the current local date and time offset.
	/// This should be used instead of <see cref="DateTimeOffset.Now"/>.
	/// </summary>
	/// <returns>The current date and time offset.</returns>
	DateTimeOffset GetTimeLocal();

	/// <summary>
	/// Gets the current UTC date and time offset.
	/// This should be used instead of <see cref="DateTimeOffset.UtcNow"/>.
	/// </summary>
	/// <returns>The current date and time offset.</returns>
	DateTimeOffset GetTimeUtc();

	/// <summary>
	/// Converts a given <see cref="string"/> to a <see cref="DateTime"/> in the configured <see cref="DateTimeKind"/>.
	/// </summary>
	/// <param name="time">The date and time in string representation.</param>
	/// <returns>The date and time.</returns>
	DateTime ToDateTime(string time);

	/// <summary>
	/// Converts a given <see cref="string"/> to a <see cref="DateTimeOffset"/>.
	/// </summary>
	/// <param name="time">The date and time in string representation.</param>
	/// <returns>The date and time offset.</returns>
	DateTimeOffset ToDateTimeOffset(string time);

	/// <summary>
	/// Converts a given <see cref="DateTime"/> to a predefined <see cref="string"/> representation.
	/// </summary>
	/// <param name="time">The time.</param>
	/// <returns>The string representation of the time.</returns>
	string ToString(DateTime time);

	/// <summary>
	/// Converts a given <see cref="DateTimeOffset"/> to a predefined <see cref="string"/> representation.
	/// </summary>
	/// <param name="time">The time offset.</param>
	/// <returns>The string representation of the time offset.</returns>
	string ToString(DateTimeOffset time);
}
