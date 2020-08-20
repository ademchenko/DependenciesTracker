using System;
using System.Collections.Generic;

namespace DependenciesTracking
{
    internal abstract class PathItemBase<T>
    {
        public PathItemBase<T> Ancestor { get; }

        public Action<T> UpdateDependentPropertyOrFieldAction { get; }

        protected PathItemBase(PathItemBase<T> ancestor, Action<T> updateDependentPropertyOrFieldAction)
        {
            Ancestor = ancestor;
            UpdateDependentPropertyOrFieldAction = updateDependentPropertyOrFieldAction;
        }

        internal IEnumerable<string> PathStrings
        {
            get
            {
                yield return StringRep;

                if (Ancestor != null)
                {
                    foreach (var pathString in Ancestor.PathStrings)
                        yield return pathString;
                }
            }
        }

        protected abstract string StringRep { get; }

        public override string ToString() => string.Join("->", PathStrings);
    }
}