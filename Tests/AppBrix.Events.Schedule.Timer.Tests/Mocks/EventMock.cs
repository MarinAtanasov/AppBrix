// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;

namespace AppBrix.Events.Schedule.Timer.Tests.Mocks;

/// <summary>
/// A simple event storing an integer value.
/// Used for testing purposes.
/// </summary>
internal sealed class EventMock : IEvent
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="EventMock"/>.
    /// </summary>
    /// <param name="value">The value to store.</param>
    public EventMock(int value)
    {
        this.Value = value;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the stored value.
    /// </summary>
    public int Value { get; }
    #endregion
}
