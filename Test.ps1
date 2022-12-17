param([String]$tests="Functional", [switch]$parallel, [switch]$build, [switch]$release);

[xml]$properties = Get-Content Directory.Build.props;
$framework = $properties.Project.PropertyGroup.TargetFramework;

$configuration = "Debug";
if ($release)
{
    $configuration = "Release";
}

if ($build)
{
    dotnet build AppBrix.sln --configuration $($configuration) --nologo --verbosity minimal;
}

$filter = "";
if ($tests -eq "functional" -or $tests -eq "f")
{
    $filter = "--TestCaseFilter:Category=Functional";
}
elseif ($tests -eq "performance" -or $tests -eq "p")
{
    $filter = "--TestCaseFilter:Category=Performance";
}

$execute = "";
if ($parallel)
{
    $execute = "--Parallel";
}

$paths = Get-ChildItem Tests -Directory | % { Join-Path $_.FullName -ChildPath ("bin/$($configuration)/$($framework)/$($_.Name).dll") };
dotnet test $paths $filter $execute --nologo --verbosity minimal;
