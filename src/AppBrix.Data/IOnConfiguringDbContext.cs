// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// An event which is fired during <see cref="DbContextBase.OnConfiguring"/>.
    /// </summary>
    public interface IOnConfiguringDbContext : IEvent
    {
        DbContextOptionsBuilder OptionsBuilder { get; }
    }
}
