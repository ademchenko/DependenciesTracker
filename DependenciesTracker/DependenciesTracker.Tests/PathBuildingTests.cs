using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Extensions;

namespace DependenciesTracker.Tests.PathBuilding
{
    public interface IPathBuildingTestClass
    {
        int IntProperty { get; set; }
    }

    public class PathBuildingTestClass : IPathBuildingTestClass
    {
        public int IntProperty { get; set; }

        public int DependentProperty { get; set; }

        public List<string> Strings { get; set; }

        public List<int> Ints { get; set; }

        public string StringProperty { get; set; }

        public PathBuildingInnerTestClass InnerProperty { get; set; }

        public PathBuildingTestClass Child { get; set; }
    }

    public class PathBuildingCollectionTestClass<T> : ObservableCollection<T>
    {
        public int IntProperty { get; set; }
    }

    public class PathBuildingInnerTestClass
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
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

        public static IEnumerable<Expression<Func<PathBuildingTestClass, object>>[]> AddDependency_ConvertsInsideThePath_NotSupported_TestData
        {
            get
            {
                //Only single convert to "object" is allowed in the end of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => (decimal)o.DependentProperty };
                //Only single convert to "object" is allowed in the end of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => (object)(decimal)o.DependentProperty };
                //Only single convert to "object" is allowed in the end of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => (IPathBuildingTestClass)o.Child };
                //Only single convert to "object" is allowed in the end of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => (object)(IPathBuildingTestClass)o.Child };
                //No converts are allowed at the begginning of path (i.e. root object convertion isn't allowed)
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => ((IPathBuildingTestClass)o).IntProperty };
                //No converts are allowed in the middle of path
                yield return new Expression<Func<PathBuildingTestClass, object>>[] { o => ((IPathBuildingTestClass)o.Child).IntProperty };
            }
        }

        [Theory]
        [PropertyData("AddDependency_RootOnlyPath_NotSupported_TestData")]
        [PropertyData("AddDependency_ExternalMethodCallsInPath_NotSupported_TestData")]
        [PropertyData("AddDependency_ConvertsInsideThePath_NotSupported_TestData")]
        public void AddDependency_NotSupportedPaths(Expression<Func<PathBuildingTestClass, object>> path)
        {
            var map = new DependenciesMap<PathBuildingTestClass>();

            Assert.Throws<NotSupportedException>(() =>
                map.AddMap(o => o.DependentProperty, o => -1, path));
        }

        public static IEnumerable<object[]> AddDependency_ConvertToObject_AllowedAtTheEndOfPath_TestData
        {
            get
            {
                //Explicit conversion of value type
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>) (o => (object) o.IntProperty),
                    new[] {"root", "IntProperty"}
                };

                //Implicit conversion of value type
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>) (o => o.IntProperty),
                    new[] {"root", "IntProperty"}
                };

                //Explicit conversion of reference type
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>) (o => (object) o.Child),
                    new[] {"root", "Child"}
                };

                //Explicit conversion of collection
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>) (o => (object) o.Strings),
                    new[] {"root", "Strings"}
                };

                //Explicit conversion of "EachElement" call on collection of reference type elements
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>) (o => (object) DependenciesTracker.CollectionExtensions.EachElement(o.Ints)),
                    new[] {"root", "Ints", "CollectionItem"}
                };

                //Implicit conversion of "EachElement" call on collection of value type elements
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>) (o => (object) DependenciesTracker.CollectionExtensions.EachElement(o.Ints)),
                    new[] {"root", "Ints", "CollectionItem"}
                };
            }
        }

        [Theory]
        [PropertyData("AddDependency_ConvertToObject_AllowedAtTheEndOfPath_TestData")]
        public void AddDependency_ConvertToObject_AllowedAtTheEndOfPath(Expression<Func<PathBuildingTestClass, object>> path,
            string[] expectedParseResult)
        {
            SupportedPathsTestImpl(path, expectedParseResult);
        }

        public static IEnumerable<object[]> AddDependency_SupportedPaths_TestData
        {
            get
            {
                //Simple reference type property
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>)(o => o.StringProperty),
                    new[] {"root", "StringProperty"}
                };

                //Simple value type property
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>)(o => o.IntProperty),
                    new[] {"root", "IntProperty"}
                };

                //Simple reference type property chain
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>)(o => o.InnerProperty.StringProperty),
                    new[] {"root", "InnerProperty", "StringProperty"}
                };

                //Simple property chain with the value type object in the end of chain
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>)(o => o.InnerProperty.IntProperty),
                    new[] {"root", "InnerProperty", "IntProperty"}
                };

                //Property chain with collection item of reference type
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>)(o => DependenciesTracker.CollectionExtensions.EachElement(o.Strings)),
                    new[] {"root", "Strings", "CollectionItem"}
                };

                //Property chain with collection item of value type in the end of chain
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>)(o => DependenciesTracker.CollectionExtensions.EachElement(o.Ints)),
                    new[] {"root", "Ints", "CollectionItem"}
                };
                
                //Property chain with collection item in the middle of chain
                yield return new object[]
                {
                    (Expression<Func<PathBuildingTestClass, object>>)(o => DependenciesTracker.CollectionExtensions.EachElement(o.Strings).Length),
                    new[] {"root", "Strings", "CollectionItem", "Length"}
                };
            }
        }

        [Theory]
        [PropertyData("AddDependency_SupportedPaths_TestData")]
        public void AddDependency_SupportedPaths(Expression<Func<PathBuildingTestClass, object>> path,
            string[] expectedParseResult)
        {
            SupportedPathsTestImpl(path, expectedParseResult);
        }

        //Reproduces the https://github.com/ademchenko/DependenciesTracker/issues/3
        [Fact]
        public void AddDependency_PropertyChainWithSingleCollectionItemAtTheBegginning_Supported()
        {
            var map = new DependenciesMap<PathBuildingCollectionTestClass<string>>();
            map.AddMap(o => o.IntProperty, o => -1, o => DependenciesTracker.CollectionExtensions.EachElement(o));

            Assert.Equal(new[] {"root", "CollectionItem"}, map.MapItems.Single().PathStrings);
        }

        private static void SupportedPathsTestImpl(Expression<Func<PathBuildingTestClass, object>> path, string[] expectedParseResult)
        {
            var map = new DependenciesMap<PathBuildingTestClass>();

            map.AddMap(o => o.DependentProperty, o => -1, path);

            Assert.Equal(expectedParseResult, map.MapItems.Single().PathStrings);
        }
    }
}
