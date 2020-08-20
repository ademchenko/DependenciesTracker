using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DependenciesTracking
{
    public static class CollectionExtensions
    {
        public static T EachElement<T>(this ICollection<T> _) => throw new NotSupportedException("Call of this method is not supported");

        public static MethodInfo EachElementMethodInfo { get; }

        static CollectionExtensions()
        {
            Expression<Func<ICollection<object>, object>> anyElementExpression = c => c.EachElement();
            EachElementMethodInfo = ((MethodCallExpression)anyElementExpression.Body).Method.GetGenericMethodDefinition();
        }
    }
}