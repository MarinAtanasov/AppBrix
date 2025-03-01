// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

using AppBrix.Events.Contracts;

namespace AppBrix.Events.Delayed.Contracts;

/// <summary>
/// Base interface for an immediate events.
/// Sets the default behavior of the event to immediate.
/// </summary>
public interface IImmediateEvent : IEvent
{
}
