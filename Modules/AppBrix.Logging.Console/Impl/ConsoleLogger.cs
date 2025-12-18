// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using AppBrix.Logging.Events;
using System;

namespace AppBrix.Logging.Console.Impl;

/// <summary>
/// A logger which writes entries to the console.
/// </summary>
internal sealed class ConsoleLogger : IApplicationLifecycle
{
	#region Public and overriden methods
	public void Initialize(IInitializeContext context)
	{
		this.app = context.App;
		this.app.GetLogHub().Subscribe(this.LogEntry);
	}

	public void Uninitialize()
	{
		this.app?.GetLogHub().Unsubscribe(this.LogEntry);
		this.app = null;
	}
	#endregion

	#region Private methods
	private void LogEntry(ILogEntry entry) => System.Console.WriteLine(entry.ToString()?.Replace(Environment.NewLine, "\n"));
	#endregion

	#region Private fields and constants
	private IApp? app;
	#endregion
}
