// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Events.Tests.Mocks
{
    /// <summary>
    /// A child event type used for testing of hierarchical behavior.
    /// </summary>
    internal sealed class EventMockChild : EventMock
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="EventMockChild"/>.
        /// </summary>
        /// <param name="value">The value to store.</param>
        public EventMockChild(int value)
            : base(value)
        {
        }
        #endregion
    }
}
