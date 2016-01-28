using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace DependenciesTracking
{
    internal partial class DependenciesTracker<T>
    {
        private sealed class RootObjectSubscriber : SubscriberBase
        {
            public RootObjectSubscriber(object effectiveObject, PathItemBase<T> pathItem, Action<PathItemBase<T>> onChanged)
                : base(effectiveObject, pathItem, onChanged)
            {
                Ancestors.Add(CreateSubscriber(effectiveObject, pathItem.Ancestor, onChanged));
            }
        }

        private sealed class PropertyChangeSubscriber : SubscriberBase
        {
            private IDisposable _observer;

            private new PropertyPathItem<T> PathItem { get { return (PropertyPathItem<T>)base.PathItem; } }

            public PropertyChangeSubscriber(object effectiveObject, PropertyPathItem<T> pathItem, Action<PathItemBase<T>> onChanged)
                : base(effectiveObject, pathItem, onChanged)
            {
                var notifyPropertyChange = effectiveObject as INotifyPropertyChanged;

                if (notifyPropertyChange != null)
                    Subscribe(notifyPropertyChange);

                var ancestor = InitAncestor();
                if (ancestor != null)
                    Ancestors.Add(ancestor);

            }

            private ISubscriberBase InitAncestor()
            {
                if (PathItem.Ancestor == null)
                    return null;

                var ancestorEffectiveObject = PathItem.PropertyOrFieldGetter(EffectiveObject);

                return ancestorEffectiveObject == null ? null : CreateSubscriber(ancestorEffectiveObject, PathItem.Ancestor, OnChanged);
            }

            private void Subscribe(INotifyPropertyChanged notifyPropertyChange)
            {
                notifyPropertyChange.PropertyChanged += notifyPropertyChange_PropertyChanged;
                _observer = new Disposable(() => notifyPropertyChange.PropertyChanged -= notifyPropertyChange_PropertyChanged);

                //_observer = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                //    h => notifyPropertyChange.PropertyChanged += h,
                //    h => notifyPropertyChange.PropertyChanged -= h)
                //    .Where(sa => sa.EventArgs.PropertyName == PathItem.PropertyOrFieldName)
                //    .Subscribe(_ => OnObservedPropertyChanged());
            }

            private void notifyPropertyChange_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == PathItem.PropertyOrFieldName)
                    OnObservedPropertyChanged();
            }

            private void OnObservedPropertyChanged()
            {
                DisposeAndClearAncestors();
                var ancestor = InitAncestor();
                if (ancestor != null)
                    Ancestors.Add(ancestor);
                OnChanged(PathItem);
            }

            public override void Dispose()
            {
                if (_observer != null)
                    _observer.Dispose();

                base.Dispose();
            }
        }

        private sealed class CollectionChangeSubscriber : SubscriberBase
        {
            private IDisposable _observer;

            private new IEnumerable EffectiveObject
            {
                get { return (IEnumerable)base.EffectiveObject; }
            }

            private new CollectionPathItem<T> PathItem { get { return (CollectionPathItem<T>)base.PathItem; } }

            public CollectionChangeSubscriber(object effectiveObject, CollectionPathItem<T> pathItem, Action<PathItemBase<T>> onChanged)
                : base(effectiveObject, pathItem, onChanged)
            {
                var notifyCollectionChange = effectiveObject as INotifyCollectionChanged;
                if (notifyCollectionChange != null)
                    SubscribeToCollectionChange(notifyCollectionChange);

                foreach (var ancestor in InitAncestors(EffectiveObject))
                    Ancestors.Add(ancestor);
            }

            private sealed class NullCollectionItemSubscriber : ISubscriberBase
            {
                public object EffectiveObject { get { throw new NotSupportedException("Call to EffectiveObject is not supported in NullCollectionItemSubscriber"); } }

                private static readonly IList<ISubscriberBase> _emptyAncestorsList = new List<ISubscriberBase>();

                public IList<ISubscriberBase> Ancestors { get { return _emptyAncestorsList; } }

                public void Dispose() { }
            }

            private IEnumerable<ISubscriberBase> InitAncestors(IEnumerable items)
            {
                if (PathItem.Ancestor == null)
                    return new SubscriberBase[0];

                return items.Cast<object>().Select(CreateSubscriberForCollectionItem);
            }

            private void SubscribeToCollectionChange(INotifyCollectionChanged notifyCollectionChange)
            {
                Debug.Assert(notifyCollectionChange != null);

                notifyCollectionChange.CollectionChanged += notifyCollectionChange_CollectionChanged;
                _observer = new Disposable(() => notifyCollectionChange.CollectionChanged -= notifyCollectionChange_CollectionChanged);
                //_observer = Observable
                //    .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                //        h => notifyCollectionChange.CollectionChanged += h,
                //        h => notifyCollectionChange.CollectionChanged -= h)
                //    .Subscribe(ep => CollectionItemsChanged(ep.EventArgs));
            }

            private void notifyCollectionChange_CollectionChanged(object _, NotifyCollectionChangedEventArgs e)
            {
                CollectionItemsChanged(e);
            }

            private const int _invalidCollectionIndexValue = -1;

            private void CollectionItemsChanged(NotifyCollectionChangedEventArgs eventArgs)
            {
                if (PathItem.Ancestor != null)
                    UpdateAncestors(eventArgs);

                OnChanged(PathItem);
            }

            private ISubscriberBase CreateSubscriberForCollectionItem(object collectionItem)
            {
                return collectionItem == null ? new NullCollectionItemSubscriber() : (ISubscriberBase)CreateSubscriber(collectionItem, PathItem.Ancestor, OnChanged);
            }

            private void UpdateAncestors(NotifyCollectionChangedEventArgs eventArgs)
            {
                switch (eventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        CheckIndexValid(eventArgs.NewStartingIndex, "NewStartingIndex", NotifyCollectionChangedAction.Add);
                        Ancestors.Insert(eventArgs.NewStartingIndex, CreateSubscriberForCollectionItem(eventArgs.NewItems.Cast<object>().Single()));
                        return;
                    case NotifyCollectionChangedAction.Remove:
                        CheckIndexValid(eventArgs.OldStartingIndex, "OldStartingIndex", NotifyCollectionChangedAction.Remove);
                        Ancestors[eventArgs.OldStartingIndex].Dispose();
                        Ancestors.RemoveAt(eventArgs.OldStartingIndex);
                        return;
                    case NotifyCollectionChangedAction.Replace:
                        CheckIndexValid(eventArgs.OldStartingIndex, "OldStartingIndex", NotifyCollectionChangedAction.Replace);
                        Ancestors[eventArgs.OldStartingIndex].Dispose();
                        Ancestors[eventArgs.OldStartingIndex] = CreateSubscriberForCollectionItem(eventArgs.NewItems.Cast<object>().Single());
                        return;
                    case NotifyCollectionChangedAction.Move:
                        CheckIndexValid(eventArgs.OldStartingIndex, "OldStartingIndex", NotifyCollectionChangedAction.Move);
                        CheckIndexValid(eventArgs.NewStartingIndex, "NewStartingIndex", NotifyCollectionChangedAction.Move);
                        var movingAncestor = Ancestors[eventArgs.OldStartingIndex];
                        Ancestors.RemoveAt(eventArgs.OldStartingIndex);
                        Ancestors.Insert(eventArgs.NewStartingIndex, movingAncestor);
                        return;
                    case NotifyCollectionChangedAction.Reset:
                        DisposeAndClearAncestors();
                        foreach (var ancestor in InitAncestors(EffectiveObject))
                            Ancestors.Add(ancestor);
                        return;
                    default:
                        throw new ArgumentException(string.Format("Unknown eventArgs.Action enum action: {0}", eventArgs.Action), "eventArgs");
                }
            }

            private static void CheckIndexValid(int index, string indexName, NotifyCollectionChangedAction action)
            {
                if (index == _invalidCollectionIndexValue)
                    throw new NotSupportedException(string.Format("Processing {0} with unset {1} is not supported", action, indexName));
            }

            public override void Dispose()
            {
                if (_observer != null)
                    _observer.Dispose();

                base.Dispose();
            }
        }

        private interface ISubscriberBase : IDisposable
        {
            object EffectiveObject { get; }

            IList<ISubscriberBase> Ancestors { get; }
        }

        private abstract class SubscriberBase : ISubscriberBase
        {
            private readonly object _effectiveObject;

            protected readonly PathItemBase<T> PathItem;

            protected readonly Action<PathItemBase<T>> OnChanged;

            private readonly IList<ISubscriberBase> _ancestors = new List<ISubscriberBase>();

            public object EffectiveObject
            {
                get { return _effectiveObject; }
            }

            public IList<ISubscriberBase> Ancestors
            {
                get { return _ancestors; }
            }

            public static SubscriberBase CreateSubscriber(object effectiveObject, PathItemBase<T> pathItem, Action<PathItemBase<T>> onChanged)
            {
                var propertyPathItem = pathItem as PropertyPathItem<T>;
                if (propertyPathItem != null)
                {
                    if (propertyPathItem.PropertyOrFieldName == string.Empty)
                        return new RootObjectSubscriber(effectiveObject, pathItem, onChanged);

                    return new PropertyChangeSubscriber(effectiveObject, propertyPathItem, onChanged);
                }

                var collectionPathItem = pathItem as CollectionPathItem<T>;
                if (collectionPathItem != null)
                    return new CollectionChangeSubscriber(effectiveObject, collectionPathItem, onChanged);

                throw new ArgumentException(string.Format("Unknown path item type: {0}", pathItem.GetType()), "pathItem");
            }

            protected SubscriberBase(object effectiveObject, PathItemBase<T> pathItem, Action<PathItemBase<T>> onChanged)
            {
                if (effectiveObject == null)
                    throw new ArgumentNullException("effectiveObject");
                if (pathItem == null)
                    throw new ArgumentNullException("pathItem");
                if (onChanged == null)
                    throw new ArgumentNullException("onChanged");

                _effectiveObject = effectiveObject;
                PathItem = pathItem;
                OnChanged = onChanged;
            }

            public virtual void Dispose()
            {
                DisposeAndClearAncestors();
            }

            protected void DisposeAndClearAncestors()
            {
                foreach (var ancestor in Ancestors)
                    ancestor.Dispose();

                Ancestors.Clear();
            }
        }
    }
}
