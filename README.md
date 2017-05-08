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
# Restore nuget dependencies.
dotnet restore
# Build solution.
dotnet build AppBrix.sln
```

## Running Tests
The tests are using xUnit.
* In Visual Studio, Test Explorer must be opened before building test projects in order to discover the tests.
* In Project Rider, they can be run with right click on *Tests* solution folder and selecting *Run Unit Tests*.
* In PowerShell, while in the project's root folder:
```Powershell
# Build solution
dotnet build AppBrix.sln
# Run functional tests in parallel (default)
./Test -Tests=Functional
# Run performance tests
./Test -Tests=Performance
# Run all tests
./Test -Tests=All
```

## Sample Applications
AppBrix.ConsoleApp is a simple console application which uses the framework.

AppBrix.WebApp is a simple web application which uses the framework.
