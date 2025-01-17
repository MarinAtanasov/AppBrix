# AppBrix
[![Build](https://github.com/MarinAtanasov/AppBrix/actions/workflows/build.yml/badge.svg)](https://github.com/MarinAtanasov/AppBrix/actions/workflows/build.yml)
[![GitHub Issues](https://img.shields.io/github/issues/MarinAtanasov/AppBrix.svg?style=flat&logo=github&label=Issues)](https://github.com/MarinAtanasov/AppBrix/issues)
[![Code Size](https://img.shields.io/github/languages/code-size/MarinAtanasov/AppBrix.svg?style=flat&logo=github&label=Code)](https://github.com/MarinAtanasov/AppBrix)
[![Repo Size](https://img.shields.io/github/repo-size/MarinAtanasov/AppBrix.svg?style=flat&logo=github&label=Repo)](https://github.com/MarinAtanasov/AppBrix)
[![NuGet Package](https://img.shields.io/nuget/v/AppBrix.svg?style=flat&logo=nuget&color=success&label=Nuget)](https://www.nuget.org/packages?q=Owner%3Amarin.atanasov.bg+Tags%3AAppBrix)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AppBrix.svg?style=flat&logo=nuget&label=Downloads)](https://www.nuget.org/packages?q=Owner%3Amarin.atanasov.bg+Tags%3AAppBrix)

AppBrix is a way of thinking about and building scalable modular applications.

This framework is using the latest version of [.NET](https://dotnet.microsoft.com/download).

AppBrix has been created with the following priorities:
* Everything should be testable.
* Interfaces should be easy to understand and use.
* Every part of the framework should be easily replaceable without changing the original source code.

## Setup
```Bash
# Clone project locally.
git clone git@github.com:MarinAtanasov/AppBrix.git
# Go to project root directory.
cd AppBrix
# Restore nuget dependencies and build the solution.
dotnet build
```

## Run Tests
By default, tests are using MSTest, but NUnit and Xunit can be used instead.
* In [Visual Studio](https://visualstudio.microsoft.com/), Test Explorer must be opened before building test projects in order to discover the tests.
* In [JetBrains Rider](https://www.jetbrains.com/rider/), they can be run with right click on *Tests* solution folder and selecting *Run Unit Tests*.
* In [PowerShell](https://github.com/PowerShell/PowerShell), while in the project's root folder:
```Powershell
# You can add -Build to restore dependencies and build the solution.
# You can add -Release to use the Release configuration instead of Debug.
# Run functional tests (default). Add -Parallel for parallel execution.
./Test.ps1 -tests Functional  # ./Test.ps1
# Run performance tests
./Test.ps1 -tests Performance  # ./Test.ps1 p
# Run all tests
./Test.ps1 -tests All  # ./Test.ps1 a
```

## Switch Test Runners
There are ready configuration for each test runner inside _/Tests/Directory.Build.props_.

If you wish to switch to a different runner, open the file and:
1. Comment out the _ItemGroup_ for the current provider.
2. Uncomment the _ItemGroup_ for the desired provider.
3. Rebuild the solution. If it doesn't work, run `./Clean.ps1` and then rebuild.

## Publishing packages
Packaging and publishing of a new version of all projects can be done using PowerShell.
It requires Nuget CLI to be set up locally with account API key.
```Powershell
./Publish.ps1
```

## Sample Applications
AppBrix.ConsoleApp is a simple console application which uses the framework.

AppBrix.WebApp is a simple web application which uses the framework.
