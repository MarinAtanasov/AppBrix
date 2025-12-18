// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using AppBrix.Logging.Events;
using System.IO;
using System.Threading;

namespace AppBrix.Logging.File.Impl;

/// <summary>
/// A log writer which writes entries to the console.
/// </summary>
internal sealed class FileLogger : IApplicationLifecycle
{
	#region Public and overriden methods
	public void Initialize(IInitializeContext context)
	{
		this.app = context.App;
		var config = this.app.ConfigService.GetFileLoggerConfig();
		this.writer = System.IO.File.AppendText(config.Path);
		this.writer.AutoFlush = true;
		this.app.GetLogHub().Subscribe(this.LogEntry);
	}

	public void Uninitialize()
	{
		this.app?.GetLogHub().Unsubscribe(this.LogEntry);
		this.app = null!;
		this.writer?.Dispose();
		this.writer = null!;
	}
	#endregion

	#region Private methods
	private void LogEntry(ILogEntry entry)
	{
		if (this.writer is not null)
		{
			lock (this.logLock)
			{
				this.writer.WriteLine(entry.ToString());
			}
		}
	}
	#endregion

	#region Private fields and constants
	private readonly Lock logLock = new Lock();
	private IApp? app;
	private StreamWriter? writer;
	#endregion
}
