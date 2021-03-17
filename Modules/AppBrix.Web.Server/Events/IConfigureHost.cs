// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events;
using Microsoft.Extensions.Hosting;

namespace AppBrix.Web.Server.Events
{
    /// <summary>
    /// An event which is called when the <see cref="IApp"/> is being attached to the <see cref="IHost"/>.
    /// </summary>
    public interface IConfigureHost : IEvent
    {
        /// <summary>
        /// Gets the builder for <see cref="IHost"/>.
        /// </summary>
        IHostBuilder Builder { get; }
    }
}
