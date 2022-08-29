using System;

namespace DependenciesTracking
{
    internal class PropertyPathItem<T> : PathItemBase<T>
    {
        private readonly Func<object, object> _propertyOrFieldGetter;

        private readonly string _propertyOrFieldName;

        public PropertyPathItem(Func<object, object> propertyOrFieldGetter, string propertyOrFieldName,
                                PathItemBase<T> ancestor, Action<T> updateDependentPropertyOrFieldAction)
            : base(ancestor, updateDependentPropertyOrFieldAction)
        {
            _propertyOrFieldGetter = propertyOrFieldGetter;
            _propertyOrFieldName = propertyOrFieldName;
        }

        public Func<object, object> PropertyOrFieldGetter
        {
            get { return _propertyOrFieldGetter; }
        }

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