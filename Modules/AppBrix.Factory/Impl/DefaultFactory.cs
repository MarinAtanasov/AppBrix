// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Factory.Impl
{
    internal interface IDefaultFactory<out T> : IFactory<T>
    {
        Func<object> Factory { get; set; }
    }

    internal class DefaultFactory<T> : IDefaultFactory<T>
    {
        public DefaultFactory(Func<object> factory)
        {
            this.Factory = factory;
        }

        public Func<object> Factory { get; set; }

        public T Get() => (T)this.Factory();
    }
}
