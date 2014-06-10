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
        private readonly IList<PropertyChangeSubscriber> _rootSubscribers = new List<PropertyChangeSubscriber>();

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
                _rootSubscribers.Add(Subscribe(map, _trackedObject));
                ProvokeDependentPropertiesUpdate(map);
            }
        }

        private void OnPropertyChanged([NotNull] PropertyChangeSubscriber subscriber)
        {
            subscriber.Ancestor = Subscribe(subscriber.PathItem.Ancestor, subscriber.PathItem.PropertyGetter(subscriber.EffectiveObject));
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


        private PropertyChangeSubscriber Subscribe(PathItem<T> pathItem, object trackedObject)
        {
            if (pathItem == null || trackedObject == null)
                return null;

            var subscriber = new PropertyChangeSubscriber(trackedObject, pathItem, OnPropertyChanged);
            subscriber.Ancestor = Subscribe(pathItem.Ancestor, pathItem.PropertyGetter(trackedObject));
            return subscriber;
        }


        public void Dispose()
        {
            foreach (var rootSubscriber in _rootSubscribers)
                rootSubscriber.Dispose();
        }
    }
}
