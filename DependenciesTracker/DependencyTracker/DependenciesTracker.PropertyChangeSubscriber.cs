using System;
using System.ComponentModel;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    internal partial class DependenciesTracker<T>
    {
        private class PropertyChangeSubscriber<T> : IDisposable
        {
            [NotNull]
            private readonly object _effectiveObject;
            [NotNull]
            private readonly PathItem<T> _pathItem;
            [NotNull]
            private readonly Action<PropertyChangeSubscriber<T>> _onChanged;
            [CanBeNull]
            private IDisposable _observer;

            public PropertyChangeSubscriber([NotNull] object effectiveObject,
                [NotNull] PathItem<T> pathItem, [NotNull] Action<PropertyChangeSubscriber<T>> onChanged)
            {
                if (effectiveObject == null) throw new ArgumentNullException("effectiveObject");
                if (pathItem == null) throw new ArgumentNullException("pathItem");
                if (onChanged == null) throw new ArgumentNullException("onChanged");

                _effectiveObject = effectiveObject;
                _pathItem = pathItem;
                _onChanged = onChanged;

                var notifyPropertyChange = effectiveObject as INotifyPropertyChanged;

                if (notifyPropertyChange != null)
                    Subscribe(notifyPropertyChange);
            }

            [CanBeNull]
            public PropertyChangeSubscriber<T> Ancestor { get; set; }

            [NotNull]
            public PathItem<T> PathItem
            {
                get { return _pathItem; }
            }

            [NotNull]
            public object EffectiveObject
            {
                get { return _effectiveObject; }
            }

            private void Subscribe([NotNull] INotifyPropertyChanged notifyPropertyChange)
            {
                _observer = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => notifyPropertyChange.PropertyChanged += h,
                    h => notifyPropertyChange.PropertyChanged -= h)
                    .Where(sa => sa.EventArgs.PropertyName == PathItem.PropertyName)
                    .Subscribe(_ => _onChanged(this));
            }


            public void Dispose()
            {
                if (_observer != null)
                    _observer.Dispose();
                if (Ancestor != null)
                    Ancestor.Dispose();
            }
        }
    }
}
