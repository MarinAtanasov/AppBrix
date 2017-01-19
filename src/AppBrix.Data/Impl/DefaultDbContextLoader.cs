// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace AppBrix.Data.Impl
{
    internal sealed class DefaultDbContextLoader : IDbContextLoader, IApplicationLifecycle
    {
        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.app = null;
        }

        public DbContext Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!typeof(DbContext).GetTypeInfo().IsAssignableFrom(type))
                throw new ArgumentException($"Provided type must inherit from {typeof(DbContext)}.");
            if (type.GetTypeInfo().IsAbstract)
                throw new ArgumentException($"Cannot create instance of abstract type {type}.");
            
            var context = (DbContext)this.app.GetFactory().Get(type);
            if (context is DbContextBase)
                ((DbContextBase)context).Initialize(new DefaultInitializeDbContext(this.app, null));
            return context;
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
