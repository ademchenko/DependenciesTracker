using System;

namespace DependenciesTracking
{
    internal class CollectionPathItem<T> : PathItemBase<T>
    {
        public CollectionPathItem(PathItemBase<T> ancestor, Action<T> updateDependentPropertyOrFieldAction)
            : base(ancestor, updateDependentPropertyOrFieldAction) { }

        protected override string StringRep
        {
            get { return "CollectionItem"; }
        }
    }
}