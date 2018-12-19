param([String]$Tests="Functional", [switch]$Parallel, [switch]$Build);

$paths = Get-ChildItem Tests -Directory | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp2.2/$($_.Name).dll") };
$filter = "";
$execute = "";

if ($Tests -eq "functional" -or $Tests -eq "f")
{
    $filter = "--TestCaseFilter:Category=Functional";
}
elseif ($Tests -eq "performance" -or $Tests -eq "p")
{
    $filter = "--TestCaseFilter:Category=Performance";
}

if ($Parallel)
{
    $execute = "--Parallel";
}

if ($Build)
{
    dotnet build AppBrix.sln;
}

dotnet vstest $paths $filter $execute;
