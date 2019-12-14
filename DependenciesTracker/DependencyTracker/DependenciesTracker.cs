using System;
using System.Collections.Generic;

namespace DependenciesTracking
{
    internal partial class DependenciesTracker<T> : IDisposable
    {
        private readonly DependenciesMap<T> _map;

        private readonly T _trackedObject;

        private readonly IList<ISubscriberBase> _rootSubscribers = new List<ISubscriberBase>();

        public DependenciesTracker(DependenciesMap<T> map, T trackedObject)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (trackedObject == null)
                throw new ArgumentNullException(nameof(trackedObject));

            _map = map;
            _trackedObject = trackedObject;

            StartTrackingChanges();
        }

        private void StartTrackingChanges()
        {
            foreach (var map in _map.MapItems)
            {
                _rootSubscribers.Add(SubscriberBase.CreateSubscriber(_trackedObject, map, OnPropertyChanged));
                ProvokeDependentPropertiesUpdate(map);
            }
        }

        private void OnPropertyChanged(PathItemBase<T> subscriber)
        {
            ProvokeDependentPropertiesUpdate(subscriber);
        }

        private void ProvokeDependentPropertiesUpdate(PathItemBase<T> pathItem)
        {
            while (pathItem != null)
            {
                pathItem.UpdateDependentPropertyOrFieldAction?.Invoke(_trackedObject);

                pathItem = pathItem.Ancestor;
            }
        }


        public void Dispose()
        {
            foreach (var rootSubscriber in _rootSubscribers)
                rootSubscriber.Dispose();
        }
    }
}
