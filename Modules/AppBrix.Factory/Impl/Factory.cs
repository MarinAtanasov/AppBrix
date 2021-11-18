// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Factory.Impl;

internal class Factory<T> : IFactory<T>
{
    public Factory(Func<object> factory)
    {
        this.factory = factory;
    }

    public T Get() => (T)this.factory();

    private readonly Func<object> factory;
}
