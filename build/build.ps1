$baseDir = (Get-Item $PSScriptRoot).Parent.FullName

$versionFile = [IO.Path]::Combine($baseDir,'version.txt')
$releasefolder = [IO.Path]::Combine($baseDir,'SQLiteNetExtensions','bin','Release')
$nuspecFile = [IO.Path]::Combine($baseDir,'sqliteextensions.nuspec')

Set-Location $baseDir

$version = Get-Content $versionFile

dotnet build --configuration Release

dotnet test

dotnet pack --configuration Release /p:Version=$version /p:NuspecFile=$nuspecFile

Set-Location $releasefolder

dotnet nuget push *.nupkg --api-key $env:NUGET_TOKEN  --source https://api.nuget.org/v3/index.json --skip-duplicate --no-symbols