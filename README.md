# AppBrix
AppBrix is a way of thinking about and building scalable modular applications.

This framework has been written using the latest version of [.NET Core](https://www.microsoft.com/net/).

AppBrix has been created with the following priorities:
* Everything should be testable and covered with tests.
* Interfaces and their default implementations should be as simple as possible.
* Interfaces should be easy to understand and use.
* Nothing should be dependent on the default implementations.
* Every part of the framework should be easily replaceable without changing the original source code.

## Setup
* Clone project locally.
```
git clone https://github.com/MarinAtanasov/AppBrix.NetCore.git
```
* Go to project root directory.
```
cd AppBrix.NetCore
```
* Restore nuget dependencies.
```
dotnet restore
```
* Build all projects.
```
ForEach ($folder in (Get-ChildItem -Path src -Directory)) { dotnet build $folder.FullName }
```

## Running Tests
The tests are using xUnit.
* In Visual Studio, they require the Test Explorer to be opened before building the test projects in order to discover the tests.
* In Project Rider, they can be ran with right click on *Tests* solution folder and selecting *Run Unit Tests*.
* In PowerShell, they can be run using the following command while in the project's root folder:
```
ForEach ($folder in (Get-ChildItem -Path test -Directory)) { dotnet test $folder.FullName }
```

## Sample Application
AppBrix.ConsoleApp is a simple console application which uses the framework.
