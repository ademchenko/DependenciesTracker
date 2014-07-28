using System;
using JetBrains.Annotations;

namespace DependenciesTracking
{
    internal class CollectionPathItem<T> : PathItemBase<T>
    {
        public CollectionPathItem([CanBeNull] PathItemBase<T> ancestor, [CanBeNull] Action<T> updateDependentPropertyOrFieldAction)
            : base(ancestor, updateDependentPropertyOrFieldAction) { }

        protected override string StringRep
        {
            get { return "CollectionItem"; }
        }
    }
}