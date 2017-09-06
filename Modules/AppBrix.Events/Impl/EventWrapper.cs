// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Impl
{
    internal sealed class EventWrapper
    {
        public EventWrapper(object handler, Action<object> execute)
        {
            this.Handler = handler;
            this.Execute = execute;
        }

        public object Handler { get; }
        
        public Action<object> Execute { get; }
    }
}
