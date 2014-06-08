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
        private readonly IList<PathItem<T>> _mapItems = new List<PathItem<T>>();

        [NotNull]
        private readonly ReadOnlyCollection<PathItem<T>> _readOnlyMapItems;

        [NotNull]
        internal ReadOnlyCollection<PathItem<T>> MapItems
        {
            get { return _readOnlyMapItems; }
        }

        public DependenciesMap()
        {
            _readOnlyMapItems = new ReadOnlyCollection<PathItem<T>>(_mapItems);
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
        private static IEnumerable<PathItem<T>> BuildPaths([NotNull] IEnumerable<Expression<Func<T, object>>> dependencyPaths,
            [NotNull] Action<T> calculateAndSet)
        {
            if (calculateAndSet == null)
                throw new ArgumentNullException("calculateAndSet");

            return dependencyPaths.Select(pathExpression => BuildPath(pathExpression, calculateAndSet)).ToList();
        }

        [NotNull]
        private static PathItem<T> BuildPath([NotNull] Expression<Func<T, object>> pathExpession, Action<T> calculateAndSet)
        {
            var convertExpression = pathExpession.Body as UnaryExpression;
            if (convertExpression != null && convertExpression.NodeType != ExpressionType.Convert)
                throw new InvalidOperationException("unary expression is not a convert expression");

            var memberExpression = (MemberExpression)(convertExpression != null ? convertExpression.Operand : pathExpession.Body);
            Debug.WriteLine(memberExpression.Expression.Type);

            PathItem<T> rootPathItem = null;

            while (memberExpression != null)
            {
                var property = memberExpression.Member;
                var compiledGetter = BuildGetter(memberExpression.Expression.Type, property.Name);

                rootPathItem = new PathItem<T>(compiledGetter, property.Name, rootPathItem, rootPathItem == null ? calculateAndSet : null);

                if (!(memberExpression.Expression is MemberExpression) && !(memberExpression.Expression is ParameterExpression))
                    throw new InvalidOperationException(string.Format("The expression {0} should be either member or parameter expression", memberExpression.Expression));

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            Debug.Assert(rootPathItem != null);

            return rootPathItem;
        }

        [NotNull]
        private static Func<object, object> BuildGetter([NotNull] Type parameterType, [NotNull] string propertyName)
        {
            var parameter = Expression.Parameter(typeof(object), "obj");
            var convertedParameter = Expression.Convert(parameter, parameterType);
            var propertyGetter = Expression.Property(convertedParameter, propertyName);
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