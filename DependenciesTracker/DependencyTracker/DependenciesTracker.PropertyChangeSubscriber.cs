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

            private new PropertyPathItem<T> PathItem => (PropertyPathItem<T>)base.PathItem;

            public PropertyChangeSubscriber(object effectiveObject, PropertyPathItem<T> pathItem, Action<PathItemBase<T>> onChanged)
                : base(effectiveObject, pathItem, onChanged)
            {
                if (effectiveObject is INotifyPropertyChanged notifyPropertyChange)
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
                _observer?.Dispose();

                base.Dispose();
            }
        }

        private sealed class CollectionChangeSubscriber : SubscriberBase
        {
            private IDisposable _observer;

            private new IEnumerable EffectiveObject => (IEnumerable)base.EffectiveObject;

            private new CollectionPathItem<T> PathItem => (CollectionPathItem<T>)base.PathItem;

            public CollectionChangeSubscriber(object effectiveObject, CollectionPathItem<T> pathItem, Action<PathItemBase<T>> onChanged)
                : base(effectiveObject, pathItem, onChanged)
            {
                if (effectiveObject is INotifyCollectionChanged notifyCollectionChange)
                    SubscribeToCollectionChange(notifyCollectionChange);

                foreach (var ancestor in InitAncestors(EffectiveObject))
                    Ancestors.Add(ancestor);
            }

            private sealed class NullCollectionItemSubscriber : ISubscriberBase
            {
                public object EffectiveObject => throw new NotSupportedException("Call to EffectiveObject is not supported in NullCollectionItemSubscriber");

                private static readonly IList<ISubscriberBase> _emptyAncestorsList = new List<ISubscriberBase>();

                public IList<ISubscriberBase> Ancestors => _emptyAncestorsList;

                public void Dispose() { }
            }

            private IEnumerable<ISubscriberBase> InitAncestors(IEnumerable items) => 
                PathItem.Ancestor == null ? new SubscriberBase[0] : items.Cast<object>().Select(CreateSubscriberForCollectionItem);

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
                        throw new ArgumentException($"Unknown eventArgs.Action enum action: {eventArgs.Action}", nameof(eventArgs));
                }
            }

            private static void CheckIndexValid(int index, string indexName, NotifyCollectionChangedAction action)
            {
                if (index == _invalidCollectionIndexValue)
                    throw new NotSupportedException($"Processing {action} with unset {indexName} is not supported");
            }

            public override void Dispose()
            {
                _observer?.Dispose();

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
            protected readonly PathItemBase<T> PathItem;

            protected readonly Action<PathItemBase<T>> OnChanged;

            public object EffectiveObject { get; }

            public IList<ISubscriberBase> Ancestors { get; } = new List<ISubscriberBase>();

            public static SubscriberBase CreateSubscriber(object effectiveObject, PathItemBase<T> pathItem, Action<PathItemBase<T>> onChanged)
            {
                if (pathItem is PropertyPathItem<T> propertyPathItem)
                {
                    if (propertyPathItem.PropertyOrFieldName == string.Empty)
                        return new RootObjectSubscriber(effectiveObject, pathItem, onChanged);

                    return new PropertyChangeSubscriber(effectiveObject, propertyPathItem, onChanged);
                }

                if (pathItem is CollectionPathItem<T> collectionPathItem)
                    return new CollectionChangeSubscriber(effectiveObject, collectionPathItem, onChanged);

                throw new ArgumentException($"Unknown path item type: {pathItem.GetType()}", nameof(pathItem));
            }

            protected SubscriberBase(object effectiveObject, PathItemBase<T> pathItem, Action<PathItemBase<T>> onChanged)
            {
                if (effectiveObject == null)
                    throw new ArgumentNullException(nameof(effectiveObject));
                if (pathItem == null)
                    throw new ArgumentNullException(nameof(pathItem));
                if (onChanged == null)
                    throw new ArgumentNullException(nameof(onChanged));

                EffectiveObject = effectiveObject;
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
