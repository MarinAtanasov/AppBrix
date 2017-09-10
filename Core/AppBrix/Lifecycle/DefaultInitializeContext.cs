// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    internal sealed class DefaultInitializeContext : IInitializeContext
    {
        #region Construction
        public DefaultInitializeContext(IApp app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            this.App = app;
        }
        #endregion

        #region Properties
        public IApp App { get; }
        #endregion
    }
}
