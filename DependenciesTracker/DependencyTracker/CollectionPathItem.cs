using System;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    internal class CollectionPathItem<T> : PathItemBase<T>
    {
        public CollectionPathItem([CanBeNull] PathItemBase<T> ancestor, [CanBeNull] Action<T> updateDependentPropertyAction)
            : base(ancestor, updateDependentPropertyAction) { }

        protected override string StringRep
        {
            get { return "CollectionItem"; }
        }
    }
}