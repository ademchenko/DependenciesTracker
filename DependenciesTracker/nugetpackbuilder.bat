rmdir /s /q "NuGetPackaging"
xcopy /Y DependenciesTracker.Interfaces\bin\Debug\DependenciesTracking.Interfaces.dll NuGetPackaging\DependenciesTracker.Interfaces\lib\portable-net45+netcore45+wp8+MonoAndroid1+MonoTouch1\*
xcopy /Y DependenciesTracking.Interfaces.Net40\bin\Debug\DependenciesTracking.Interfaces.dll NuGetPackaging\DependenciesTracker.Interfaces\lib\net40\*
xcopy /Y DependenciesTracker.Interfaces\DependenciesTracking.Interfaces.nuspec NuGetPackaging\DependenciesTracker.Interfaces\*
cd NuGetPackaging
nuget pack DependenciesTracker.Interfaces\DependenciesTracking.Interfaces.nuspec
for %%a in (DependenciesTracking.Interfaces.*.nupkg) do nuget push "%%a"
ren "*.nupkg" "*.nupkg-pushed"

cd ..
xcopy /Y DependencyTracker\bin\Debug\DependenciesTracking.dll NuGetPackaging\DependenciesTracker\lib\portable-net45+netcore45+wp8+MonoAndroid1+MonoTouch1\*
xcopy /Y DependenciesTracking.Net40\bin\Debug\DependenciesTracking.dll NuGetPackaging\DependenciesTracker\lib\net40\*
xcopy /Y DependencyTracker\DependenciesTracking.nuspec NuGetPackaging\DependenciesTracker\*
cd NuGetPackaging
nuget pack DependenciesTracker\DependenciesTracking.nuspec
for %%a in (DependenciesTracking.*.nupkg) do nuget push "%%a"
ren "*.nupkg" "*.nupkg-pushed"

cd..
