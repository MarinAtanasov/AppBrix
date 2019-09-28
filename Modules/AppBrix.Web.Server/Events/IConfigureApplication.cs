// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events;
using Microsoft.AspNetCore.Builder;

namespace AppBrix.Web.Server.Events
{
    /// <summary>
    /// An event which is called during WebHost's Configure method.
    /// </summary>
    public interface IConfigureApplication : IEvent
    {
        /// <summary>
        /// Gets the class that provides the mechanisms to configure an application's request pipeline.
        /// </summary>
        IApplicationBuilder Builder { get; }
    }
}
