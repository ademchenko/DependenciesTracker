using System;
using JetBrains.Annotations;

namespace DependenciesTracking
{
    internal class PropertyPathItem<T> : PathItemBase<T>
    {
        [NotNull]
        private readonly Func<object, object> _propertyOrFieldGetter;

        [NotNull]
        private readonly string _propertyOrFieldName;

        public PropertyPathItem([NotNull] Func<object, object> propertyOrFieldGetter, [NotNull] string propertyOrFieldName,
            [CanBeNull] PathItemBase<T> ancestor, [CanBeNull] Action<T> updateDependentPropertyOrFieldAction)
            : base(ancestor, updateDependentPropertyOrFieldAction)
        {
            _propertyOrFieldGetter = propertyOrFieldGetter;
            _propertyOrFieldName = propertyOrFieldName;
        }

        [NotNull]
        public Func<object, object> PropertyOrFieldGetter
        {
            get { return _propertyOrFieldGetter; }
        }

        [NotNull]
        public string PropertyOrFieldName
        {
            get { return _propertyOrFieldName; }
        }

        protected override string StringRep
        {
            get { return PropertyOrFieldName != string.Empty ? PropertyOrFieldName : "root"; }
        }
    }
}