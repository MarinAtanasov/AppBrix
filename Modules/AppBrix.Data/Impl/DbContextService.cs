﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Data;
using AppBrix.Data.Services;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppBrix.Data.Impl;

internal sealed class DbContextService : IDbContextService, IApplicationLifecycle
{
    #region Public and overriden methods
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
    }

    public void Uninitialize()
    {
        this.app = null!;
    }

    public DbContext Get(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        var factory = this.app.GetFactoryService().GetFactory(type);
        var context = factory?.Get() ?? type.CreateObject();
        (context as DbContextBase)?.Initialize(new InitializeDbContext(this.app));
        return (DbContext)context;
    }
    #endregion

    #region Private fields and constants
    private IApp app = null!;
    #endregion
}
