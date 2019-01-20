# AppBrix
AppBrix is a way of thinking about and building scalable modular applications.

This framework is using the latest version of [.NET Core](https://www.microsoft.com/net/core).

AppBrix has been created with the following priorities:
* Everything should be testable and covered with tests.
* Interfaces should be easy to understand and use.
* Every part of the framework should be easily replaceable without changing the original source code.

## Setup
```Bash
# Clone project locally.
git clone git@github.com:MarinAtanasov/AppBrix.NetCore.git
# Go to project root directory.
cd AppBrix.NetCore
# Restore nuget dependencies and build the solution.
dotnet build AppBrix.sln
```

## Running Tests
The tests are using xUnit.
* In Visual Studio, Test Explorer must be opened before building test projects in order to discover the tests.
* In Project Rider, they can be run with right click on *Tests* solution folder and selecting *Run Unit Tests*.
* In PowerShell, while in the project's root folder:
```Powershell
# You can add -Build to restore dependencies and build the solution.
# You can add -Release to use the Release configuration instead of Debug.
# Run functional tests (default). Add -Parallel for parallel execution.
./Test -Tests Functional # ./Test f
# Run performance tests
./Test -Tests Performance # ./Test p
# Run all tests
./Test -Tests All # ./Test a
```

## Publishing packages
Packaging and publishing of a new version of all projects can be done using PowerShell.
It requires Nuget CLI to be set up locally with account API key.
```Powershell
./Publish 1.0.0
```

## Sample Applications
AppBrix.ConsoleApp is a simple console application which uses the framework.

AppBrix.WebApp is a simple web application which uses the framework.
