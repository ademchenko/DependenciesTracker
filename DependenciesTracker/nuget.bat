cd "DependenciesTracker.Interfaces"
del /Q "*.nupkg"
nuget pack "DependenciesTracking.Interfaces.csproj" -IncludeReferencedProjects
for %%a in (*.nupkg) do nuget push "%%a"

cd ..
cd "DependenciesTracker"
del /Q "*.nupkg"
nuget pack "DependenciesTracking.csproj" -IncludeReferencedProjects
for %%a in (*.nupkg) do nuget push "%%a"