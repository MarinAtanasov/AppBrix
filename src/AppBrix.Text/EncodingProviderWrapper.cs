// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using System.Text;
using AppBrix.Application;
using AppBrix.Lifecycle;

namespace AppBrix.Text
{
    /// <summary>
    /// Used as a bridge between the statically registered encoding provider and the application encoding provider.
    /// </summary>
    internal sealed class EncodingProviderWrapper : EncodingProvider, IApplicationLifecycle
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

        public override Encoding GetEncoding(string name)
        {
            return app.Get<EncodingProvider>().GetEncoding(name);
        }

        public override Encoding GetEncoding(int codepage)
        {
            return app.Get<EncodingProvider>().GetEncoding(codepage);
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
