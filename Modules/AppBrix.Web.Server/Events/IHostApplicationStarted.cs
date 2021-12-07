// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;

namespace AppBrix.Web.Server.Events;

/// <summary>
/// An event which is called after the host application has started.
/// </summary>
public interface IHostApplicationStarted : IEvent
{
}
