using System.Collections.Generic;

namespace DependenciesTracker
{
    public static class CollectionExtensions
    {
        public static T AnyElement<T>(this ICollection<T> collection)
        {
            return default(T);
        }
    }
}