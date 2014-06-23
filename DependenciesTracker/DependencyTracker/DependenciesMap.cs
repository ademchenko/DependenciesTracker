using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using DependenciesTracker.Interfaces;
using JetBrains.Annotations;


namespace DependenciesTracker
{
    public sealed class DependenciesMap<T> : IDependenciesMap<T>
    {
        [NotNull]
        private readonly IList<PathItemBase<T>> _mapItems = new List<PathItemBase<T>>();

        [NotNull]
        private readonly ReadOnlyCollection<PathItemBase<T>> _readOnlyMapItems;

        [NotNull]
        internal ReadOnlyCollection<PathItemBase<T>> MapItems
        {
            get { return _readOnlyMapItems; }
        }

        public DependenciesMap()
        {
            _readOnlyMapItems = new ReadOnlyCollection<PathItemBase<T>>(_mapItems);
        }

        [NotNull]
        public IDependenciesMap<T> AddMap<U>([NotNull] Action<T, U> setter, [NotNull] Func<T, U> calculator, [NotNull] params Expression<Func<T, object>>[] dependencyPaths)
        {
            if (setter == null)
                throw new ArgumentNullException("setter");
            if (calculator == null)
                throw new ArgumentNullException("calculator");

            foreach (var builtPath in BuildPaths(dependencyPaths, o => setter(o, calculator(o))))
                _mapItems.Add(builtPath);

            return this;
        }

        [NotNull]
        public IDependenciesMap<T> AddMap<U>([NotNull] Expression<Func<T, U>> dependentProperty, [NotNull] Func<T, U> calculator, [NotNull] params Expression<Func<T, object>>[] dependencyPaths)
        {
            if (dependentProperty == null)
                throw new ArgumentNullException("dependentProperty");

            return AddMap(BuildSetter(dependentProperty), calculator, dependencyPaths);
        }

        [NotNull]
        private static Action<T, U> BuildSetter<U>([NotNull] Expression<Func<T, U>> dependentProperty)
        {
            var memberExpression = (MemberExpression)dependentProperty.Body;
            var objectParameter = Expression.Parameter(typeof(T), "obj");
            var assignParameter = Expression.Parameter(typeof(U), "val");
            var property = Expression.PropertyOrField(objectParameter, memberExpression.Member.Name);
            var lambda = Expression.Lambda<Action<T, U>>(Expression.Assign(property, assignParameter), objectParameter, assignParameter);
            Debug.WriteLine(lambda);
            return lambda.Compile();
        }

        [NotNull]
        private static IEnumerable<PathItemBase<T>> BuildPaths([NotNull] IEnumerable<Expression<Func<T, object>>> dependencyPaths,
            [NotNull] Action<T> calculateAndSet)
        {
            if (calculateAndSet == null)
                throw new ArgumentNullException("calculateAndSet");

            return dependencyPaths.Select(pathExpression => BuildPath(pathExpression, calculateAndSet)).ToList();
        }

        [NotNull]
        private static PathItemBase<T> BuildPath([NotNull] Expression<Func<T, object>> pathExpession, Action<T> calculateAndSet)
        {
            var convertExpression = pathExpession.Body as UnaryExpression;
            if (convertExpression != null &&
                (convertExpression.NodeType != ExpressionType.Convert || convertExpression.Type != typeof(object)))
                throw new NotSupportedException(string.Format(
                    "Unary expression {0} is not supported. Only \"convert to object\" expression is allowed in the end of path.", convertExpression));

            var currentExpression = convertExpression != null ? convertExpression.Operand : pathExpession.Body;

            PathItemBase<T> rootPathItem = null;

            while (!(currentExpression is ParameterExpression))
            {
                var methodCall = currentExpression as MethodCallExpression;
                if (methodCall != null)
                {
                    if (!methodCall.Method.IsGenericMethod || !methodCall.Method.GetGenericMethodDefinition().Equals(CollectionExtensions.EachElementMethodInfo))
                        throw new NotSupportedException(string.Format("Call of method {0} is not supported. Only {1} call is supported for collections in path",
                                                                      methodCall.Method, CollectionExtensions.EachElementMethodInfo));

                    rootPathItem = new CollectionPathItem<T>(rootPathItem, rootPathItem == null ? calculateAndSet : null);

                    var methodCallArgument = methodCall.Arguments.Single();
                    currentExpression = methodCallArgument;
                    continue;
                }

                var memberExpression = currentExpression as MemberExpression;
                if (memberExpression == null)
                    throw new NotSupportedException(string.Format("Expected expression is member expression. Expression {0} is not supported.", currentExpression));

                var property = memberExpression.Member;
                var compiledGetter = BuildGetter(memberExpression.Expression.Type, property.Name);

                rootPathItem = new PropertyPathItem<T>(compiledGetter, property.Name, rootPathItem, rootPathItem == null ? calculateAndSet : null);

                currentExpression = memberExpression.Expression;
            }

            //The chain doesn't contain any element (i.e. the expression contains only root object root => root)
            if (rootPathItem == null)
                throw new NotSupportedException(string.Format("The path {0} is too short. It contains a root object only.", pathExpession));

            rootPathItem = new PropertyPathItem<T>(o => o, string.Empty, rootPathItem, null);

            return rootPathItem;
        }

        [NotNull]
        private static Func<object, object> BuildGetter([NotNull] Type parameterType, [NotNull] string propertyOrFieldName)
        {
            var parameter = Expression.Parameter(typeof(object), "obj");
            var convertedParameter = Expression.Convert(parameter, parameterType);
            var propertyGetter = Expression.PropertyOrField(convertedParameter, propertyOrFieldName);
            
            Debug.WriteLine(propertyGetter);

            var lambdaExpression = Expression.Lambda<Func<object, object>>(Expression.Convert(propertyGetter, typeof(object)), parameter);
            Debug.WriteLine(lambdaExpression);
            return lambdaExpression.Compile();

        }

        public IDisposable StartTracking([NotNull] T trackedObject)
        {
            return new DependenciesTracker<T>(this, trackedObject);
        }
    }
}