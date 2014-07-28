using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DependenciesTracking
{
    internal partial class DependenciesTracker<T> : IDisposable
    {
        [NotNull]
        private readonly DependenciesMap<T> _map;
        [NotNull]
        private readonly T _trackedObject;
        [NotNull]
        private readonly IList<ISubscriberBase> _rootSubscribers = new List<ISubscriberBase>();

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
                _rootSubscribers.Add(SubscriberBase.CreateSubscriber(_trackedObject, map, OnPropertyChanged));
                ProvokeDependentPropertiesUpdate(map);
            }
        }

        private void OnPropertyChanged([NotNull] PathItemBase<T> subscriber)
        {   
            ProvokeDependentPropertiesUpdate(subscriber);
        }

        private void ProvokeDependentPropertiesUpdate([CanBeNull] PathItemBase<T> pathItem)
        {
            while (pathItem != null)
            {
                if (pathItem.UpdateDependentPropertyOrFieldAction != null)
                    pathItem.UpdateDependentPropertyOrFieldAction(_trackedObject);

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
