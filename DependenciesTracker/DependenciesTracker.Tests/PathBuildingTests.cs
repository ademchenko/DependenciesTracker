using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Extensions;

namespace DependenciesTracker.Tests.PathBuilding
{
    public interface IPathBuildingTestClass {
        int IntProperty { get; set; }
    }

    public class PathBuildingTestClass : IPathBuildingTestClass
    {
        public int IntProperty { get; set; }

        public int DependentProperty { get; set; }

        public List<string> Strings { get; set; }

        public PathBuildingTestClass Parent { get; set; }
    }

    public static class IntegerExtension
    {
        public static string ExtensionCall(this int i)
        {
            throw new NotImplementedException();
        }
    }

    public static class CollectionExtensions
    {
        public static T EachElement<T>(this ICollection<T> collection)
        {
            throw new NotSupportedException("Call of this method is not supported");
        }
    }


    public class PathBuildingTests
    {
        public static IEnumerable<Expression<Func<PathBuildingTestClass, object>>[]> AddDependency_RootOnlyPath_NotSupported_TestData
        {
            get
            {
                //With convert
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => (object)o };
                //With no convert
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => o };
            }
        }

        [Theory, PropertyData("AddDependency_RootOnlyPath_NotSupported_TestData")]
        public void AddDependency_RootOnlyPath_NotSupported(Expression<Func<PathBuildingTestClass, object>> path)
        {
            NotSupportedPathTestImpl(path);
        }


        public static IEnumerable<Expression<Func<PathBuildingTestClass, object>>[]> AddDependency_ExternalMethodCallsInPath_NotSupported_TestData
        {
            get
            {
                //Instance method of simple property
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => o.IntProperty.ToString().Length };
                //Extension method of simple property
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => o.IntProperty.ExtensionCall().Length };
                //Instance method of collection property
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => o.Strings.ToArray().Length };
                //Extension method of collection property
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => o.Strings.First().Length };
                //Extension method of collection property with the same name of class and method as DependenciesTracker.CollectionExtensions.EachElement<T>
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => o.Strings.EachElement().Length };
            }
        }

        [Theory, PropertyData("AddDependency_ExternalMethodCallsInPath_NotSupported_TestData")]
        public void AddDependency_ExternalMethodCallsInPath_NotSupported(Expression<Func<PathBuildingTestClass, object>> path)
        {
            NotSupportedPathTestImpl(path);
        }

        public static IEnumerable<Expression<Func<PathBuildingTestClass, object>>[]> AddDependency_ConvertsInsideThePath_NotSupported_TestData
        {
            get
            {
                //Only convert to "object" is allowed in the end of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => (decimal)o.DependentProperty };
                //Only convert to "object" is allowed in the end of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => (IPathBuildingTestClass)o.Parent };
                //No converts are allowed at the begginning of path (i.e. root object convertion isn't allowed)
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => ((IPathBuildingTestClass)o).IntProperty };
                //No converts are allowed in the middle of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => ((IPathBuildingTestClass)o.Parent).IntProperty };
            }
        }

        [Theory, PropertyData("AddDependency_ConvertsInsideThePath_NotSupported_TestData")]
        public void AddDependency_ConvertsInsideThePath_NotSupported(Expression<Func<PathBuildingTestClass, object>> path)
        {
            NotSupportedPathTestImpl(path);
        }

        private static void NotSupportedPathTestImpl(Expression<Func<PathBuildingTestClass, object>> path)
        {
            var map = new DependenciesMap<PathBuildingTestClass>();

            Assert.Throws<NotSupportedException>(() =>
                map.AddMap(o => o.DependentProperty, o => -1, path));
        }
    }
}
