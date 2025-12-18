// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace AppBrix.Web.Server.Tests.Mocks;

/// <summary>
/// A simple controller which can be used to test sending and receiving data.
/// </summary>
[Route("api/[controller]")]
public sealed class EchoController : Controller
{
	#region Public and overriden methods
	/// <summary>
	/// Echoes the text that has been sent.
	/// </summary>
	/// <returns></returns>
	[HttpGet("{text}")]
	public string Echo(string text) => text;

	/// <summary>
	/// Echoes the object that has been sent
	/// </summary>
	/// <returns></returns>
	[HttpPost]
	public EchoModel Echo([FromBody] EchoModel model) => model;
	#endregion
}
