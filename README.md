# AppBrix
AppBrix is a way of thinking about and building scalable modular applications.

This framework has been written using the latest version of [.NET Core](https://www.microsoft.com/net/core).

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
* In Visual Studio, they require the Test Explorer to be opened before building the test projects in order to discover the tests.
* In Project Rider, they can be ran with right click on *Tests* solution folder and selecting *Run Unit Tests*.
* In PowerShell, while in the project's root folder:
```Powershell
# Build solution
dotnet build AppBrix.sln
# Run functional tests in parallel
dotnet vstest (Get-ChildItem Tests | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp1.1/$($_.Name).dll") }) --TestCaseFilter:Category=Functional --Parallel
# Run performance tests
dotnet vstest (Get-ChildItem Tests | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp1.1/$($_.Name).dll") }) --TestCaseFilter:Category=Performance
# Run all tests
dotnet vstest (Get-ChildItem Tests | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp1.1/$($_.Name).dll") })
```

## Sample Applications
AppBrix.ConsoleApp is a simple console application which uses the framework.

AppBrix.WebApp is a simple web application which uses the framework.
