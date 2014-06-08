using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    internal partial class DependenciesTracker<T> : IDisposable
    {
        [NotNull]
        private readonly DependenciesMap<T> _map;
        [NotNull]
        private readonly T _trackedObject;
        [NotNull]
        private readonly IList<PropertyChangeSubscriber<T>> _rootSubscribers = new List<PropertyChangeSubscriber<T>>();

        public DependenciesTracker([NotNull] DependenciesMap<T> map, [NotNull] T trackedObject)
        {
            if (map == null)
                throw new ArgumentNullException("map");
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (trackedObject == null)
                throw new ArgumentNullException("trackedObject");

            _map = map;
            _trackedObject = trackedObject;

            StartTrackingChanges();
        }

        private void StartTrackingChanges()
        {
            foreach (var map in _map.MapItems)
            {
                _rootSubscribers.Add(Subscribe(map, _trackedObject, null));
                ProvokeDependentPropertiesUpdate(map);
            }
        }

        private void OnPropertyChanged([NotNull] PropertyChangeSubscriber<T> subscriber)
        {
            subscriber.Ancestor = Subscribe(subscriber.PathItem.Ancestor, subscriber.PathItem.PropertyGetter(subscriber.EffectiveObject), subscriber);
            ProvokeDependentPropertiesUpdate(subscriber.PathItem);
        }

        private void ProvokeDependentPropertiesUpdate([CanBeNull] PathItem<T> pathItem)
        {
            if (pathItem == null)
                return;

            if (pathItem.UpdateDependentPropertyAction != null)
                pathItem.UpdateDependentPropertyAction(_trackedObject);

            ProvokeDependentPropertiesUpdate(pathItem.Ancestor);
        }


        private PropertyChangeSubscriber<T> Subscribe(PathItem<T> pathItem, object trackedObject,
            [CanBeNull] PropertyChangeSubscriber<T> parent)
        {
            if (pathItem == null || trackedObject == null)
                return null;
            
            var subscriber = new PropertyChangeSubscriber<T>(trackedObject, pathItem, OnPropertyChanged);
            subscriber.Ancestor = Subscribe(pathItem.Ancestor, pathItem.PropertyGetter(trackedObject), subscriber);
            return subscriber;
        }


        public void Dispose()
        {
            foreach (var rootSubscriber in _rootSubscribers)
                rootSubscriber.Dispose();
        }
    }
}
