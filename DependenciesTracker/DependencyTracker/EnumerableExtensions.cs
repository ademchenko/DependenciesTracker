using System;
using System.Collections.Generic;

namespace DependenciesTracking
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> StartWith<T>(this IEnumerable<T> source, T startingElement)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            yield return startingElement;
            
            foreach (var element in source)
                yield return element;
        }
    }
}