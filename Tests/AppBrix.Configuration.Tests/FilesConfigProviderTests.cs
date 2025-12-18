// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Files;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using System;
using System.IO;
using System.Reflection;

namespace AppBrix.Configuration.Tests;

[TestClass]
public sealed class FilesConfigProviderTests : TestsBase
{
	#region Tests
	[Test, Functional]
	public void TestConstructorNullSerializer()
	{
		var action = () => new FilesConfigProvider(null!, "test_dir");
		this.AssertThrows<ArgumentNullException>(action, "serializer cannot be null");;
	}

	[Test, Functional]
	public void TestConstructorNullDirectory()
	{
		var action = () => new FilesConfigProvider(new ConfigSerializerMock(), null!);
		this.AssertThrows<ArgumentNullException>(action, "directory cannot be null");;
	}

	[Test, Functional]
	public void TestConstructorEmptyDirectory()
	{
		var action = () => new FilesConfigProvider(new ConfigSerializerMock(), string.Empty);
		this.AssertThrows<ArgumentNullException>(action, "directory cannot be empty");;
	}

	[Test, Functional]
	public void TestConstructorNullFileExtension()
	{
		var action = () => new FilesConfigProvider(new ConfigSerializerMock(), "test_dir", null!);
		this.AssertThrows<ArgumentNullException>(action, "fileExtension cannot be null");;
	}

	[Test, Functional]
	public void TestConstructorEmptyFileExtension()
	{
		var action = () => new FilesConfigProvider(new ConfigSerializerMock(), "test_dir", string.Empty);
		this.AssertThrows<ArgumentNullException>(action, "fileExtension cannot be empty");;
	}

	[Test, Functional]
	public void TestGetConfigNullType()
	{
		var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
		var provider = new FilesConfigProvider(new ConfigSerializerMock(), directory);
		var action = () => provider.Get(null!);
		this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
	}

	[Test, Functional]
	public void TestSaveConfigNullConfig()
	{
		var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
		var provider = new FilesConfigProvider(new ConfigSerializerMock(), directory);
		var action = () => provider.Save(((IConfig)null)!);
		this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
	}
	#endregion
}
