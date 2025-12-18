// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

using AppBrix.Factory.Contracts;

namespace AppBrix.Factory.Tests.Mocks;

internal class FactoryMock<T> : IFactory<T>
{
	public FactoryMock(T value)
	{
		this.Value = value;
	}

	public T Value { get; }

	public T Get() => this.Value;
}
