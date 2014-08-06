@echo off &setlocal
cd "DependenciesTracker.Interfaces"
nuget spec -f

set "licensetoken=<licenseUrl>http://LICENSE_URL_HERE_OR_DELETE_THIS_LINE</licenseUrl>"
set "icontoken=<licenseUrl>http://LICENSE_URL_HERE_OR_DELETE_THIS_LINE</licenseUrl>"
set "projecturltoken=http://PROJECT_URL_HERE_OR_DELETE_THIS_LINE"
set 

set "textfile=DependenciesTracking.Interfaces.nuspec"
(for /f "delims=" %%i in (%textfile%) do (
	set "line=%%i"
	setlocal enabledelayedexpansion
	
	set "line=!line:%licensetoken%=!"
	set "line=!line:%projecturltoken%=https://github.com/ademchenko/DependenciesTracker!"
	
	echo(!line!
	endlocal
)) > "temp.txt"

rem nuget pack "DependenciesTracking.Interfaces.csproj" -IncludeReferencedProjects
rem  nuget push "DependenciesTracking.Interfaces.1.0.0.0.nupkg"
rem cd ..
rem cd "DependenciesTracker"
rem nuget spec -f
rem nuget pack "DependenciesTracking.csproj" -IncludeReferencedProjects
rem nuget push "DependenciesTracking.1.0.0.0.nupkg"



