// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;

namespace AppBrix.Web.Server
{
    /// <summary>
    /// An event which is called when the <see cref="IApp"/> is being attached to the <see cref="IWebHost"/>.
    /// </summary>
    public interface IConfigureWebHost : IEvent
    {
        /// <summary>
        /// Gets the builder for <see cref="IWebHost"/>.
        /// </summary>
        IWebHostBuilder Builder { get; }
    }
}
