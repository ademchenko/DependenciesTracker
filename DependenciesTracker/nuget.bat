xcopy /Y DependenciesTracker.Interfaces\bin\Debug\DependenciesTracking.Interfaces.dll C:\Sources\My\DependenciesTracker\DependenciesTracker\DependenciesTracker.Interfaces.NuGet\lib\net45\*

rem cd "DependenciesTracker.Interfaces"

rem del /Q "*.nupkg"
rem nuget pack "DependenciesTracking.Interfaces.csproj" -IncludeReferencedProjects
rem for %%a in (*.nupkg) do nuget push "%%a"

rem cd ..
rem cd "DependencyTracker"
rem del /Q "*.nupkg"
rem nuget pack "DependenciesTracking.csproj" -IncludeReferencedProjects
rem for %%a in (*.nupkg) do nuget push "%%a"