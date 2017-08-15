// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AppBrix.Web.Server
{
    internal sealed class DefaultLoggerProvider : ILoggerProvider, IApplicationLifecycle
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

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DefaultLogger(this.app, categoryName);
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
