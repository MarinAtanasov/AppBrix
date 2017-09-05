// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppBrix.Data.Impl
{
    internal sealed class DefaultDbContextService : IDbContextService, IApplicationLifecycle
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
            if (!typeof(DbContext).IsAssignableFrom(type))
                throw new ArgumentException($"Provided type must inherit from {typeof(DbContext).GetAssemblyQualifiedName()}.");
            if (type.IsAbstract)
                throw new ArgumentException($"Cannot create instance of abstract type {type.GetAssemblyQualifiedName()}.");
            
            var context = (DbContext)this.app.GetFactory().Get(type);
            (context as DbContextBase)?.Initialize(new DefaultInitializeDbContext(this.app));
            return context;
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
