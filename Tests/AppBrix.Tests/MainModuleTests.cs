// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Testing;
using AppBrix.Testing.Modules;
using AppBrix.Tests.Mocks;

namespace AppBrix.Tests;

[TestClass]
public sealed class MainModuleTests : TestsBase
{
    #region Tests
    [Test, Functional]
    public void TestConfigureAddsDependencies()
    {
        var modules = this.App.ConfigService.GetAppConfig().Modules;
        modules.Add(ModuleConfigElement.Create<MainModule<SimpleModuleMock, InstallableModuleMock>>());

        this.App.Restart();

        this.Assert(modules.Count == 3, "all dependencies should be in the config");
        this.Assert(modules[0].Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "the first dependency should be first");
        this.Assert(modules[1].Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName(), "the second dependency should be second");
        this.Assert(modules[2].Type == typeof(MainModule<SimpleModuleMock, InstallableModuleMock>).GetAssemblyQualifiedName(), "the main module should be last");
    }

    [Test, Functional]
    public void TestConfigureInsertsDependenciesKeepsFirstModules()
    {
        var modules = this.App.ConfigService.GetAppConfig().Modules;
        modules.Add(ModuleConfigElement.Create<SimpleModuleMock>());
        modules.Add(ModuleConfigElement.Create<MainModule<InstallableModuleMock>>());

        this.App.Restart();

        this.Assert(modules.Count == 3, "all dependencies should be in the config");
        this.Assert(modules[0].Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "the existing module should not move");
        this.Assert(modules[1].Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName(), "the dependency should added");
        this.Assert(modules[2].Type == typeof(MainModule<InstallableModuleMock>).GetAssemblyQualifiedName(), "the main module should be last");
    }

    [Test, Functional]
    public void TestConfigureInsertsDependenciesKeepsLastModules()
    {
        var modules = this.App.ConfigService.GetAppConfig().Modules;
        modules.Add(ModuleConfigElement.Create<MainModule<InstallableModuleMock>>());
        modules.Add(ModuleConfigElement.Create<SimpleModuleMock>());

        this.App.Restart();

        this.Assert(modules.Count == 3, "all dependencies should be in the config");
        this.Assert(modules[0].Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName(), "the dependency should added");
        this.Assert(modules[1].Type == typeof(MainModule<InstallableModuleMock>).GetAssemblyQualifiedName(), "the main module should be last");
        this.Assert(modules[2].Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "the existing module should not move");
    }

    [Test, Functional]
    public void TestConfigureInsertsMissingDependencies()
    {
        var modules = this.App.ConfigService.GetAppConfig().Modules;
        modules.Add(ModuleConfigElement.Create<SimpleModuleMock>());
        modules.Add(ModuleConfigElement.Create<MainModule<SimpleModuleMock, InstallableModuleMock>>());

        this.App.Restart();

        this.Assert(modules.Count == 3, "all dependencies should be in the config");
        this.Assert(modules[0].Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "the first dependency should be first");
        this.Assert(modules[1].Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName(), "the second dependency should be second");
        this.Assert(modules[2].Type == typeof(MainModule<SimpleModuleMock, InstallableModuleMock>).GetAssemblyQualifiedName(), "the main module should be last");
    }

    [Test, Functional]
    public void TestConfigureFixesDependencyOrder()
    {
        var modules = this.App.ConfigService.GetAppConfig().Modules;
        modules.Add(ModuleConfigElement.Create<InstallableModuleMock>());
        modules.Add(ModuleConfigElement.Create<SimpleModuleMock>());
        modules.Add(ModuleConfigElement.Create<MainModule<SimpleModuleMock, InstallableModuleMock>>());

        this.App.Restart();

        this.Assert(modules.Count == 3, "all dependencies should be in the config");
        this.Assert(modules[0].Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "the first dependency should be first");
        this.Assert(modules[1].Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName(), "the second dependency should be second");
        this.Assert(modules[2].Type == typeof(MainModule<SimpleModuleMock, InstallableModuleMock>).GetAssemblyQualifiedName(), "the main module should be last");
    }
    #endregion
}
