// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using AppBrix.Time.Services;
using System;
using System.Globalization;

namespace AppBrix.Time.Impl;

internal sealed class TimeService : IApplicationLifecycle, ITimeService
{
	#region IApplicationLifecycle implementation
	public void Initialize(IInitializeContext context)
	{
		this.dateTimeFormat = context.App.ConfigService.GetTimeConfig().DateTimeFormat;
		this.offsetFormat = context.App.ConfigService.GetTimeConfig().OffsetFormat;
	}

	public void Uninitialize()
	{
		this.dateTimeFormat = string.Empty;
		this.offsetFormat = string.Empty;
	}
	#endregion

	#region ITimeService implementation
	public DateTime GetTime() => DateTime.UtcNow;

	public DateTimeOffset GetTimeLocal() => DateTimeOffset.Now;

	public DateTimeOffset GetTimeUtc() => DateTimeOffset.UtcNow;

	public DateTime ToDateTime(string time) =>
		DateTime.ParseExact(time, this.dateTimeFormat, CultureInfo.InvariantCulture).ToUniversalTime();

	public DateTimeOffset ToDateTimeOffset(string time) =>
		DateTimeOffset.ParseExact(time, this.offsetFormat, CultureInfo.InvariantCulture);

	public string ToString(DateTime time) => time.ToString(this.dateTimeFormat);

	public string ToString(DateTimeOffset time) => time.ToString(this.offsetFormat);
	#endregion

	#region Private fields and constants
	private string dateTimeFormat = string.Empty;
	private string offsetFormat = string.Empty;
	#endregion
}
