// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Text.Impl;
using System;
using System.Linq;
using System.Text;

namespace AppBrix.Text
{
    /// <summary>
    /// A module which is used for enhanced text and encoding support.
    /// </summary>
    public sealed class TextModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            this.App.GetContainer().Register(CodePagesEncodingProvider.Instance);
            this.wrapper.Value.Initialize(context);
            Encoding.RegisterProvider(this.wrapper.Value);
        }

        protected override void UninitializeModule()
        {
            this.wrapper.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<EncodingProviderWrapper> wrapper = new Lazy<EncodingProviderWrapper>();
        #endregion
    }
}
