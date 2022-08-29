cd DependencyTracker\bin\Release
dir
for %%a in (DependenciesTracking.*.nupkg) do dotnet nuget push "%%a" --api-key <puth the key here> --source  https://api.nuget.org/v3/index.json
ren "*.nupkg" "*.nupkg-pushed"

cd..
cd..
cd..