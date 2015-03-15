// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Tests.Mocks
{
    internal class EventMock : IEvent
    {
        public EventMock(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }
    }
}
