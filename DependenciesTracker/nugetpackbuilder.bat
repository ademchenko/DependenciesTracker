rmdir /s /q "NuGetPackaging"
xcopy /Y DependencyTracker\bin\Release\DependenciesTracking.dll NuGetPackaging\DependenciesTracker\lib\portable-net45+netcore45+wp8+MonoAndroid1+MonoTouch1\*
xcopy /Y DependenciesTracking.Net40\bin\Release\DependenciesTracking.dll NuGetPackaging\DependenciesTracker\lib\net40\*
xcopy /Y DependencyTracker\DependenciesTracking.nuspec NuGetPackaging\DependenciesTracker\*
cd NuGetPackaging
nuget pack DependenciesTracker\DependenciesTracking.nuspec
for %%a in (DependenciesTracking.*.nupkg) do nuget push "%%a" -Source https://www.nuget.org/api/v2/package
ren "*.nupkg" "*.nupkg-pushed"

cd..
