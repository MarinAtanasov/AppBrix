// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Linq;
using System.Text;

namespace AppBrix.Text.Impl
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
            return this.app?.Get<EncodingProvider>().GetEncoding(name);
        }

        public override Encoding GetEncoding(int codepage)
        {
            return this.app?.Get<EncodingProvider>().GetEncoding(codepage);
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
