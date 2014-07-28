using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace DependenciesTracking.Interfaces
{
    public interface IDependenciesMap<T>
    {
        [NotNull]
        IDependenciesMap<T> AddDependency<U>([NotNull] Action<T, U> setter, [NotNull] Func<T, U> calculator, Expression<Func<T, object>> obligatoryDependencyPath, [NotNull] params Expression<Func<T, object>>[] dependencyPaths);

        [NotNull]
        IDependenciesMap<T> AddDependency<U>([NotNull] Expression<Func<T, U>> dependentProperty, [NotNull] Func<T, U> calculator, Expression<Func<T, object>> obligatoryDependencyPath, [NotNull] params Expression<Func<T, object>>[] dependencyPaths);

        [NotNull]
        IDisposable StartTracking([NotNull] T trackedObject);
    }
}