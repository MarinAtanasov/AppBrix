// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
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
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var factory = this.app.GetFactoryService().GetFactory(type);
            var context = factory != null ? factory.Get() : type.CreateObject();
            (context as DbContextBase)?.Initialize(new DefaultInitializeDbContext(this.app));
            return (DbContext)context;
        }
        #endregion

        #region Private fields and constants
        #nullable disable
        private IApp app;
        #nullable restore
        #endregion
    }
}
