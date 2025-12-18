// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;

namespace AppBrix.Data.SqlServer.Configuration;

/// <summary>
/// Configures the SqlServer data provider.
/// </summary>
public sealed class SqlServerDataConfig : IConfig
{
	#region Construction
	/// <summary>
	/// Creates a new instance of <see cref="SqlServerDataConfig"/> with default property values.
	/// </summary>
	public SqlServerDataConfig()
	{
		this.ConnectionString = @"Server=.\sqlexpress;Database=AppBrix;Trusted_Connection=True;";
	}
	#endregion

	#region Properties
	/// <summary>
	/// Gets or sets the connection string to the SqlServer database instance.
	/// </summary>
	public string ConnectionString { get; set; }
	#endregion
}
