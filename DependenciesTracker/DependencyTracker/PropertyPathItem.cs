using System;

namespace DependenciesTracking
{
    internal class PropertyPathItem<T> : PathItemBase<T>
    {
        public PropertyPathItem(Func<object, object> propertyOrFieldGetter, string propertyOrFieldName,
                                PathItemBase<T> ancestor, Action<T> updateDependentPropertyOrFieldAction)
            : base(ancestor, updateDependentPropertyOrFieldAction)
        {
            PropertyOrFieldGetter = propertyOrFieldGetter;
            PropertyOrFieldName = propertyOrFieldName;
        }

        public Func<object, object> PropertyOrFieldGetter { get; }

        public string PropertyOrFieldName { get; }

        protected override string StringRep => PropertyOrFieldName != string.Empty ? PropertyOrFieldName : "root";
    }
}