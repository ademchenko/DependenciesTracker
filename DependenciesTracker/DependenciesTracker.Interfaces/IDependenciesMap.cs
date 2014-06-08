using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace DependenciesTracker.Interfaces
{
    public interface IDependenciesMap<T>
    {
        [NotNull]
        IDependenciesMap<T> AddMap<U>([NotNull] Action<T, U> setter, [NotNull] Func<T, U> calculator, [NotNull] params Expression<Func<T, object>>[] dependencyPaths);

        [NotNull]
        IDependenciesMap<T> AddMap<U>([NotNull] Expression<Func<T, U>> dependentProperty, [NotNull] Func<T, U> calculator, [NotNull] params Expression<Func<T, object>>[] dependencyPaths);

        [NotNull]
        IDisposable StartTracking([NotNull] T trackedObject);
    }
}