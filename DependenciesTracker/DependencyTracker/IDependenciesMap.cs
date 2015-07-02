using System;
using System.Linq.Expressions;

namespace DependenciesTracking.Interfaces
{
    /// <summary>
    /// Instances of the interface are used for adding dependencies and start their tracking.
    /// The actual implementation is <see cref="DependenciesMap{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of view model for which properties the dependencies are set up.</typeparam>
    public interface IDependenciesMap<T>
    {
        /// <summary>
        /// Adds dependency to a map of dependencies.
        /// </summary>
        /// <typeparam name="U">Type of dependent property.</typeparam>
        /// <param name="dependentPropertySetter">Setter of the dependent property (like (o, propValue) => o.Cost = propValue). Cannot be null.</param>
        /// <param name="calculator">A method that calculates the value of dependent property on a view model instance (like o => o.Price * o.Quantity). Cannot be null.</param>
        /// <param name="obligatoryDependencyPath">Expression that represents the path on which the property depends on (like o => o.Price). Cannot be null.</param>
        /// <param name="dependencyPaths">Expressions that represent additional paths on which the property depends on if any (like o => o.Quantity). Cannot be null itself as well as any of its items.</param>
        /// <returns>Self, to be able to use the fluent syntax when defining dependencies.</returns>
        /// <exception cref="ArgumentNullException">when <see cref="dependentPropertySetter"/> or <see cref="calculator"/> or <see cref="obligatoryDependencyPath"/> or <see cref="dependencyPaths"/> is null. 
        /// </exception>
        /// <exception cref="ArgumentException">when any item of <see cref="dependencyPaths"/> is null.</exception>
        /// <example>
        /// var dependencyMap = new DependenciesMap&lt;Order&gt;&#40;&#41;
        ///.AddDependency&#40;(o, propValue) => o.Cost = propValue, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity&#41;
        ///</example>
        IDependenciesMap<T> AddDependency<U>(Action<T, U> dependentPropertySetter, Func<T, U> calculator, Expression<Func<T, object>> obligatoryDependencyPath, params Expression<Func<T, object>>[] dependencyPaths);

        /// <summary>
        /// Adds dependency to a map of dependencies.
        /// </summary>
        /// <typeparam name="U">Type of dependent property.</typeparam>
        /// <param name="dependentProperty">Expression which describes the dependent property (like o => o.Cost). Cannot be null.</param>
        /// <param name="calculator">A method that calculates the value of dependent property on a view model instance (like o => o.Price * o.Quantity). Cannot be null.</param>
        /// <param name="obligatoryDependencyPath">Expression that represents the path on which the property depends on (like o => o.Price). Cannot be null.</param>
        /// <param name="dependencyPaths">Expression that represents additional paths on which the property depends on if any (like o => o.Quantity). Cannot be null itself as well as any of its items.</param>
        /// <returns>Self, to be able to use the fluent syntax when defining dependencies.</returns>
        /// <exception cref="ArgumentNullException">when <see cref="dependentProperty"/> or <see cref="calculator"/> or <see cref="obligatoryDependencyPath"/> or <see cref="dependencyPaths"/> is null. 
        /// </exception>
        /// <exception cref="ArgumentException">when any item of <see cref="dependencyPaths"/> is null.</exception>
        /// <example>
        /// var dependencyMap = new DependenciesMap&lt;Order&gt;&#40;&#41;
        ///.AddDependency&#40;o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity&#41;
        ///</example>
        IDependenciesMap<T> AddDependency<U>(Expression<Func<T, U>> dependentProperty, Func<T, U> calculator, Expression<Func<T, object>> obligatoryDependencyPath, params Expression<Func<T, object>>[] dependencyPaths);


        /// <summary>
        /// Starts tracking all the properties defined in current map.
        /// </summary>
        /// <param name="trackedObject">View model on which the properties will be tracked. Cannot be null.</param>
        /// <returns>Disposable which represents internal tracking object, dispose it to stop the tracking. Not null.</returns>
        /// <exception cref="ArgumentNullException"> when trackedObject is null.</exception>
        IDisposable StartTracking(T trackedObject);
    }
}