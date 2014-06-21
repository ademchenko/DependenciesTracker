using System;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    internal class PropertyPathItem<T> : PathItemBase<T>
    {
        [NotNull]
        private readonly Func<object, object> _propertyGetter;

        [NotNull]
        private readonly string _propertyName;

        public PropertyPathItem([NotNull] Func<object, object> propertyGetter, [NotNull] string propertyName,
            [CanBeNull] PathItemBase<T> ancestor, [CanBeNull] Action<T> updateDependentPropertyAction)
            : base(ancestor, updateDependentPropertyAction)
        {
            _propertyGetter = propertyGetter;
            _propertyName = propertyName;
        }

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

        protected override string StringRep
        {
            get { return PropertyName != string.Empty ? PropertyName : "root"; }
        }
    }
}