// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Threading;

namespace AppBrix.Utils.Contracts;

/// <summary>
/// Defines a class that releases a <see cref="SemaphoreSlim"/> on <see cref="IDisposable.Dispose"/>.
/// </summary>
public interface ISemaphoreLock : IDisposable
{
	/// <summary>
	/// Gets whether the current thread successfully entered the SemaphoreSlim.
	/// </summary>
	bool Success { get; }
}
