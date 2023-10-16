[xml]$properties = Get-Content Directory.Build.props;
$version = $properties.Project.PropertyGroup.Version;

ForEach ($category in "Core", "Modules", "Bundles", "Testing")
{
    ForEach ($folder in (Get-ChildItem -Path $category -Directory))
    {
        dotnet pack $folder.FullName --configuration Release --nologo /p:ContinuousIntegrationBuild=true;
        dotnet nuget push "$($folder.FullName)\bin\Release\$($folder.Name).$($version).nupkg";
        dotnet nuget push "$($folder.FullName)\bin\Release\$($folder.Name).$($version).snupkg";
    }
}
