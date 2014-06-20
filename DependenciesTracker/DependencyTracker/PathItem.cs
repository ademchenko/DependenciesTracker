using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    internal class PathItem<T>
    {
        [NotNull]
        private readonly Func<object, object> _propertyGetter;
        [NotNull]
        private readonly string _propertyName;
        
        private readonly bool _isCollection;
        
        [CanBeNull]
        private readonly PathItem<T> _ancestor;
        [CanBeNull]
        private readonly Action<T> _updateDependentPropertyAction;

        [NotNull]
        public Func<object, object> PropertyGetter
        {
            get { return _propertyGetter; }
        }

        [NotNull]
        public string PropertyName
        {
            get { return _propertyName; }
        }

        [CanBeNull]
        public PathItem<T> Ancestor
        {
            get { return _ancestor; }
        }

        [CanBeNull]
        public Action<T> UpdateDependentPropertyAction
        {
            get { return _updateDependentPropertyAction; }
        }

        public bool IsCollection
        {
            get { return _isCollection; }
        }

        public PathItem([NotNull] Func<object, object> propertyGetter, [NotNull] string propertyName, bool isCollection,
                        [CanBeNull] PathItem<T> ancestor, [CanBeNull] Action<T> updateDependentPropertyAction)
        {
            if (propertyGetter == null)
                throw new ArgumentNullException("propertyGetter");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            _propertyGetter = propertyGetter;
            _propertyName = propertyName;
            _isCollection = isCollection;
            _ancestor = ancestor;
            _updateDependentPropertyAction = updateDependentPropertyAction;
        }

        [NotNull]
        internal IEnumerable<string> PathStrings
        {
            get
            {
                yield return PropertyName != string.Empty ? PropertyName : "root";

                if (Ancestor != null)
                {
                    foreach (var pathString in Ancestor.PathStrings)
                        yield return pathString;
                }
            }
        }

        public override string ToString()
        {
            return string.Join("->", PathStrings);
        }
    }
}