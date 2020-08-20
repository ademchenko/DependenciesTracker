rmdir /s /q "NuGetPackaging"
xcopy /Y DependencyTracker\bin\Release\DependenciesTracking.*.nupkg NuGetPackaging\*
cd NuGetPackaging
for %%a in (DependenciesTracking.*.nupkg) do nuget push "%%a" -Source https://api.nuget.org/v3/index.json
ren "*.nupkg" "*.nupkg-pushed"

cd..
