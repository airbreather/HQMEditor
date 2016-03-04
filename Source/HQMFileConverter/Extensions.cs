using System;
using System.Collections.Generic;
using System.Linq;

namespace HQMFileConverter
{
    public static class Extensions
    {
        public static T[] NullIfEmpty<T>(this T[] array) => array.Length == 0 ? null : array;

        public static void CopyFrom<T>(this ICollection<T> collection, IEnumerable<T> source)
        {
            foreach (T item in source)
            {
                collection.Add(item);
            }
        }

        public static void CopyFrom<TSource, TResult>(this ICollection<TResult> collection, IEnumerable<TSource> source, Func<TSource, TResult> converter)
        {
            foreach (TSource item in source)
            {
                collection.Add(converter(item));
            }
        }

        public static TResult[] ConvertToArray<TSource, TResult>(this IReadOnlyCollection<TSource> items, Func<TSource, TResult> converter)
        {
            TResult[] result = new TResult[items.Count];
            using (IEnumerator<TSource> enumerator = items.GetEnumerator())
            {
                for (int i = 0; i < result.Length; i++)
                {
                    enumerator.MoveNext();
                    result[i] = converter(enumerator.Current);
                }

                return result;
            }
        }

        public static void Visit<T>(this IVisitor<T> visitor, IEnumerable<T> source, bool visitNulls)
        {
            if (!visitNulls)
            {
                source = source.Where(val => val != null);
            }

            foreach (T val in source)
            {
                visitor.Visit(val);
            }
        }
    }
}
