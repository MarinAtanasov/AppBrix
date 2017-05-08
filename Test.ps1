param([String]$Tests="Functional", [switch]$Restore, [switch]$Build);

$paths = Get-ChildItem Tests | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp1.1/$($_.Name).dll") };
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

if ($Restore)
{
    dotnet restore;
}

if ($Restore -or $Build)
{
    dotnet build AppBrix.sln;
}

dotnet vstest $paths $filter $parallel;
