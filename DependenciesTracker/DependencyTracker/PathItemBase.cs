using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    internal abstract class PathItemBase<T>
    {
        [CanBeNull]
        private readonly PathItemBase<T> _ancestor;
        [CanBeNull]
        private readonly Action<T> _updateDependentPropertyAction;

        [CanBeNull]
        public PathItemBase<T> Ancestor
        {
            get { return _ancestor; }
        }

        [CanBeNull]
        public Action<T> UpdateDependentPropertyAction
        {
            get { return _updateDependentPropertyAction; }
        }

        protected PathItemBase([CanBeNull] PathItemBase<T> ancestor, [CanBeNull] Action<T> updateDependentPropertyAction)
        {
            _ancestor = ancestor;
            _updateDependentPropertyAction = updateDependentPropertyAction;
        }

        [NotNull]
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

        public override string ToString()
        {
            return string.Join("->", PathStrings);
        }
    }
}