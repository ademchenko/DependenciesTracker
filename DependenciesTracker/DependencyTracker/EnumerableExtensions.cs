using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DependenciesTracker
{
    internal static class EnumerableExtensions
    {
        [NotNull]
        public static IEnumerable<T> StartWith<T>([NotNull] this IEnumerable<T> source, T startingElement)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            yield return startingElement;
            
            foreach (var element in source)
                yield return element;
        }
    }
}