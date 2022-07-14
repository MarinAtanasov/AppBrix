[xml]$Properties = Get-Content Directory.Build.props;
$Version = $Properties.Project.PropertyGroup.Version;

ForEach ($category in "Core", "Modules", "Bundles")
{
    ForEach ($folder in (Get-ChildItem -Path $category -Directory))
    {
        dotnet pack $folder.FullName --configuration Release --nologo /p:ContinuousIntegrationBuild=true;
        dotnet nuget push "$($folder.FullName)\bin\Release\$($folder.Name).$($Version).nupkg";
        dotnet nuget push "$($folder.FullName)\bin\Release\$($folder.Name).$($Version).snupkg";
    }
}
