// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events;

namespace AppBrix.Web.Server.Events
{
    /// <summary>
    /// An event which is called while the host application is stopping.
    /// </summary>
    public interface IHostApplicationStopping : IEvent
    {
    }
}
