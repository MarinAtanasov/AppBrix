// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using Microsoft.AspNetCore.Builder;

namespace AppBrix.Web.Server.Events;

/// <summary>
/// An event which is called when the <see cref="IApp"/> is being attached to the <see cref="WebApplicationBuilder"/>.
/// </summary>
public interface IConfigureWebAppBuilder : IEvent
{
    /// <summary>
    /// Gets the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    WebApplicationBuilder Builder { get; }
}
