param([String]$Tests="Functional", [switch]$Parallel, [switch]$Build, [switch]$Release);

$version = "net6.0"

$configuration = "Debug";
if ($Release)
{
    $configuration = "Release";
}

if ($Build)
{
    dotnet build AppBrix.sln --configuration $($configuration);
}

$filter = "";
if ($Tests -eq "functional" -or $Tests -eq "f")
{
    $filter = "--TestCaseFilter:Category=Functional";
}
elseif ($Tests -eq "performance" -or $Tests -eq "p")
{
    $filter = "--TestCaseFilter:Category=Performance";
}

$execute = "";
if ($Parallel)
{
    $execute = "--Parallel";
}

$paths = Get-ChildItem Tests -Directory | % { Join-Path $_.FullName -ChildPath ("bin/$($configuration)/$($version)/$($_.Name).dll") };
dotnet vstest $paths $filter $execute;
