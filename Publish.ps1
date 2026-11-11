[xml]$properties = Get-Content Directory.Build.props;
$version = $properties.Project.PropertyGroup.Version;

# Open browser to create and copy a new nuget api-key
Start-Process "https://www.nuget.org/account/apikeys?forceApiKeys=true";
$apiKey = Read-Host "Enter the nuget api-key";

ForEach ($category in "Core", "Modules", "Bundles", "Testing")
{
    ForEach ($folder in (Get-ChildItem -Path $category -Directory))
    {
        dotnet pack $folder.FullName --configuration Release --nologo /p:ContinuousIntegrationBuild=true;
        dotnet nuget push "$($folder.FullName)\bin\Release\$($folder.Name).$($version).nupkg" --api-key $apiKey;
        dotnet nuget push "$($folder.FullName)\bin\Release\$($folder.Name).$($version).snupkg" --api-key $apiKey;
    }
}
