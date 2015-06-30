﻿using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("Brings infrastructure for observing property paths changes and automatically updating properties that depend on that paths.")]

[assembly: AssemblyCompany("Alexander Demchenko")]
[assembly: AssemblyCopyright("Copyright ©  2015")]

[assembly: AssemblyTitle("DependenciesTracking")]
[assembly: AssemblyProduct("DependenciesTracking")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.3.0.0")]
[assembly: AssemblyFileVersion("0.3.0.0")]

[assembly: InternalsVisibleTo("DependenciesTracking.Tests")]
