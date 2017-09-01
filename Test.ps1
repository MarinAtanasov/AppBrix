param([String]$Tests="Functional", [switch]$Build);

$paths = Get-ChildItem Tests -Directory | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp2.0/$($_.Name).dll") };
$filter = "";
$parallel = "";

if ($Tests -eq "functional" -or $Tests -eq "f")
{
    $filter = "--TestCaseFilter:Category=Functional";
    $parallel = "--Parallel";
}
elseif ($Tests -eq "performance" -or $Tests -eq "p")
{
    $filter = "--TestCaseFilter:Category=Performance";
}

if ($Build)
{
    dotnet build AppBrix.sln;
}

dotnet vstest $paths $filter $parallel;
