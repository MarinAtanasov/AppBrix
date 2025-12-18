// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Lifecycle;

internal class InstallContext : IInstallContext
{
	#region Construction
	public InstallContext(IApp app, Version previousVersion)
	{
		this.App = app;
		this.PreviousVersion = previousVersion;
	}
	#endregion

	#region Properties
	public IApp App { get; }

	public Version PreviousVersion { get; }

	public RequestedAction RequestedAction { get; set; }
	#endregion
}
