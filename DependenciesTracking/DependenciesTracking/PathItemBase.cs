using System;
using System.Collections.Generic;

namespace DependenciesTracking
{
    internal abstract class PathItemBase<T>
    {
        private readonly PathItemBase<T> _ancestor;

        private readonly Action<T> _updateDependentPropertyOrFieldAction;

        public PathItemBase<T> Ancestor
        {
            get { return _ancestor; }
        }

        public Action<T> UpdateDependentPropertyOrFieldAction
        {
            get { return _updateDependentPropertyOrFieldAction; }
        }

        protected PathItemBase(PathItemBase<T> ancestor, Action<T> updateDependentPropertyOrFieldAction)
        {
            _ancestor = ancestor;
            _updateDependentPropertyOrFieldAction = updateDependentPropertyOrFieldAction;
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

        public override string ToString()
        {
            return string.Join("->", PathStrings);
        }
    }
}