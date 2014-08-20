using System;
using System.Linq.Expressions;

namespace DependenciesTracking.Interfaces
{
    public interface IDependenciesMap<T>
    {
        IDependenciesMap<T> AddDependency<U>(Action<T, U> setter, Func<T, U> calculator, Expression<Func<T, object>> obligatoryDependencyPath, params Expression<Func<T, object>>[] dependencyPaths);

        IDependenciesMap<T> AddDependency<U>(Expression<Func<T, U>> dependentProperty, Func<T, U> calculator, Expression<Func<T, object>> obligatoryDependencyPath, params Expression<Func<T, object>>[] dependencyPaths);

        IDisposable StartTracking(T trackedObject);
    }
}