// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory.Contracts;
using AppBrix.Factory.Services;
using AppBrix.Factory.Tests.Mocks;
using AppBrix.Testing;
using System;

namespace AppBrix.Factory.Tests;

[TestClass]
public sealed class FactoryTests : TestsBase<FactoryModule>
{
	#region Tests
	[Test, Functional]
	public void TestRegisterNullFactory()
	{
		var service = this.GetFactoryService();
		var action = () => service.Register(((IFactory<FactoryTests>)null)!);
		this.AssertThrows<ArgumentNullException>(action, "factory cannot be null");;
	}

	[Test, Functional]
	public void TestRegisterNullFactoryType()
	{
		var service = this.GetFactoryService();
		var factory = new FactoryMock<FactoryTests>(this);
		var action = () => service.Register(factory, null!);
		this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
	}

	[Test, Functional]
	public void TestRegisterNullFactoryMethod()
	{
		var service = this.GetFactoryService();
		var action = () => service.Register(((Func<FactoryTests>)null)!, typeof(FactoryTests));
		this.AssertThrows<ArgumentNullException>(action, "factory method cannot be null");;
	}

	[Test, Functional]
	public void TestRegisterNullFactoryMethodType()
	{
		var service = this.GetFactoryService();
		var factory = () => this;
		var action = () => service.Register(factory, null!);
		this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
	}

	[Test, Functional]
	public void TestFactoryNonRegisteredObject()
	{
		var service = this.GetFactoryService();

		this.Assert(service.GetFactory<DefaultConstructorClass>() is null, "no factory has been registered");

		Action action = () => service.Get<DefaultConstructorClass>();
		this.AssertThrows<InvalidOperationException>(action, "no factory has been registered");;
	}

	[Test, Functional]
	public void TestFactoryDefaultConstructorCall()
	{
		var factory = this.GetFactoryService();
		factory.Register(() => new DefaultConstructorClass());
		var returned1 = factory.Get<DefaultConstructorClass>();
		var returned2 = factory.Get<DefaultConstructorClass>();

		this.Assert(returned1 is not null, "the factory should return first object");
		this.Assert(returned2 is not null, "the factory should return second object");
		this.Assert(returned2 != returned1, "the factory should always return a new object");
	}

	[Test, Functional]
	public void TestFactoryNonDefaultConstructorCall()
	{
		var service = this.GetFactoryService();
		service.Register(() => new NonDefaultConstructorClass(true));
		var returned1 = service.Get<NonDefaultConstructorClass>();
		var returned2 = service.Get<NonDefaultConstructorClass>();

		this.Assert(returned1 is not null, "the factory should return first object");
		this.Assert(returned1.Value, $"first object value should be {true}");
		this.Assert(returned1.Modified == false, $"first object modified should be {false}");
		this.Assert(returned2 is not null, "the factory should return second object");
		this.Assert(returned2.Value, $"second object value should be {true}");
		this.Assert(returned2.Modified == false, $"first object modified should be {false}");
		this.Assert(returned2 != returned1, "the factory should always return a new object");
	}

	[Test, Functional]
	public void TestFactoryRegistersHierarchically()
	{
		var service = this.GetFactoryService();
		var original = new NonDefaultConstructorClass(true);
		service.Register(() => original);

		var returnedChild = service.Get<NonDefaultConstructorClass>();
		this.Assert(returnedChild is not null, "the factory should return child object");
		this.Assert(returnedChild == original, "child object should be the same as the original object");
		this.Assert(service.Get<DefaultConstructorClass>() == original, "parent object should be the same as the original object");
		this.Assert(service.Get<ITestInterface>() == original, "interface object should be the same as the original object");
	}

	[Test, Functional]
	public void TestFactoryCanBeWrapped()
	{
		var service = this.GetFactoryService();
		service.Register(() => new NonDefaultConstructorClass(true));
		var method = service.GetFactory<NonDefaultConstructorClass>()!;
		service.Register(() =>
		{
			var obj = method.Get();
			obj.Modified = true;
			return obj;
		});

		var returned = service.Get<NonDefaultConstructorClass>();
		this.Assert(returned is not null, "the factory should return child object");
		this.Assert(returned.Value, $"object value should be {true}");
		this.Assert(returned.Modified, $"object modified should be {true}");
	}

	[Test, Performance]
	public void TestPerformanceFactory() => this.AssertPerformance(this.TestPerformanceFactoryInternal);
	#endregion

	#region Private methods
	private IFactoryService GetFactoryService() => this.App.GetFactoryService();

	private void TestPerformanceFactoryInternal()
	{
		var service = this.GetFactoryService();
		FactoryTests Factory() => this;
		var type = typeof(FactoryTests);

		for (var i = 0; i < 1000; i++)
		{
			service.Register(Factory, type);
		}

		for (var i = 0; i < 400000; i++)
		{
			service.Get(type);
		}

		this.App.Reinitialize();
	}
	#endregion
}
