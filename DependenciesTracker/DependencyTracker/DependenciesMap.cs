using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using DependenciesTracking.Interfaces;

namespace DependenciesTracking
{
    public sealed class DependenciesMap<T> : IDependenciesMap<T>
    {
        private readonly IList<PathItemBase<T>> _mapItems = new List<PathItemBase<T>>();

        private readonly ReadOnlyCollection<PathItemBase<T>> _readOnlyMapItems;

        internal ReadOnlyCollection<PathItemBase<T>> MapItems
        {
            get { return _readOnlyMapItems; }
        }

        public DependenciesMap()
        {
            _readOnlyMapItems = new ReadOnlyCollection<PathItemBase<T>>(_mapItems);
        }

        public IDependenciesMap<T> AddDependency<U>(Action<T, U> dependentPropertySetter, Func<T, U> calculator, Expression<Func<T, object>> obligatoryDependencyPath, params Expression<Func<T, object>>[] dependencyPaths)
        {
            if (dependentPropertySetter == null)
                throw new ArgumentNullException("dependentPropertySetter");
            if (calculator == null)
                throw new ArgumentNullException("calculator");
            if (obligatoryDependencyPath == null)
                throw new ArgumentNullException("obligatoryDependencyPath");
            if (dependencyPaths == null) 
                throw new ArgumentNullException("dependencyPaths");
            if (dependencyPaths.Any(p => p == null))
                throw new ArgumentException("On of items in dependencyPaths is null.");

            foreach (var builtPath in BuildPaths(dependencyPaths.StartWith(obligatoryDependencyPath), o => dependentPropertySetter(o, calculator(o))))
                _mapItems.Add(builtPath);

            return this;
        }

        public IDependenciesMap<T> AddDependency<U>(Expression<Func<T, U>> dependentProperty, Func<T, U> calculator,
                                                    Expression<Func<T, object>> obligatoryDependencyPath,
                                                    params Expression<Func<T, object>>[] dependencyPaths)
        {
            if (dependentProperty == null)
                throw new ArgumentNullException("dependentProperty");

            return AddDependency(BuildSetter(dependentProperty), calculator, obligatoryDependencyPath, dependencyPaths);
        }

        private static Action<T, U> BuildSetter<U>(Expression<Func<T, U>> dependentProperty)
        {
            Debug.Assert(dependentProperty.Body != null);

            var memberExpression = dependentProperty.Body as MemberExpression;
            if (memberExpression == null)
                ThrowNotSupportedExpressionForDependentProperty(dependentProperty.Body);

            if (!(memberExpression.Expression is ParameterExpression))
                ThrowNotSupportedExpressionForDependentProperty(memberExpression);

            var objectParameter = Expression.Parameter(typeof(T), "obj");
            var assignParameter = Expression.Parameter(typeof(U), "val");
            var property = Expression.PropertyOrField(objectParameter, memberExpression.Member.Name);
            var lambda = Expression.Lambda<Action<T, U>>(Expression.Assign(property, assignParameter), objectParameter, assignParameter);
            Debug.WriteLine(lambda);
            return lambda.Compile();
        }

        private static void ThrowNotSupportedExpressionForDependentProperty(Expression notSuppportedExpression)
        {
            Debug.Assert(notSuppportedExpression != null);

            throw new NotSupportedException(
                string.Format("Expression {0} is not supported. The only property or field member expression with no chains (i.e. one level from root object) is supported.",
                    notSuppportedExpression));
        }

        private static IEnumerable<PathItemBase<T>> BuildPaths(IEnumerable<Expression<Func<T, object>>> dependencyPaths, Action<T> calculateAndSet)
        {
            if (calculateAndSet == null)
                throw new ArgumentNullException("calculateAndSet");

            return dependencyPaths.Select(pathExpression => BuildPath(pathExpression, calculateAndSet)).ToList();
        }

        private static PathItemBase<T> BuildPath(Expression<Func<T, object>> pathExpession, Action<T> calculateAndSet)
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

        private static Func<object, object> BuildGetter(Type parameterType, string propertyOrFieldName)
        {
            var parameter = Expression.Parameter(typeof(object), "obj");
            var convertedParameter = Expression.Convert(parameter, parameterType);
            var propertyGetter = Expression.PropertyOrField(convertedParameter, propertyOrFieldName);

            Debug.WriteLine(propertyGetter);

            var lambdaExpression = Expression.Lambda<Func<object, object>>(Expression.Convert(propertyGetter, typeof(object)), parameter);
            Debug.WriteLine(lambdaExpression);
            return lambdaExpression.Compile();

        }

        public IDisposable StartTracking(T trackedObject)
        {
            return new DependenciesTracker<T>(this, trackedObject);
        }
    }
}