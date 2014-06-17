using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    public static class CollectionExtensions
    {
        public static T EachElement<T>([CanBeNull] this ICollection<T> collection)
        {
            throw new NotSupportedException("Call of this method is not supported");
        }

        [NotNull] private static readonly MethodInfo _eachElementMethodInfo;

        [NotNull]
        public static MethodInfo EachElementMethodInfo
        {
            get { return _eachElementMethodInfo; }
        }

        static CollectionExtensions()
        {
            Expression<Func<ICollection<object>, object>> anyElementExpression = c => c.EachElement();
            _eachElementMethodInfo = ((MethodCallExpression) anyElementExpression.Body).Method.GetGenericMethodDefinition();
        }
    }
}