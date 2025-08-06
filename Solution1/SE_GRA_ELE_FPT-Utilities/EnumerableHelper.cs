using System.Collections;
namespace SE_GRA_ELE_FPT_Utilities
{
    public static class EnumerableHelper
    {
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBySequentially<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            var queue = new Queue<TSource>();
            var currentKey = default(TKey);
            var comparer = EqualityComparer<TKey>.Default;

            foreach (var item in source)
            {
                var key = keySelector(item);

                if (!comparer.Equals(key, currentKey))
                {
                    if (queue.Count != 0)
                    {
                        yield return new Grouping<TKey, TSource>(currentKey, queue.ToArray());
                        queue.Clear();
                    }
                    currentKey = key;
                }
                queue.Enqueue(item);
            }

            if (queue.Count != 0)
            {
                yield return new Grouping<TKey, TSource>(currentKey, queue.ToArray());
                queue.Clear();
            }
        }
    }

    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; }
        protected IEnumerable<TElement> Values { get; }

        public Grouping(TKey key, IEnumerable<TElement> values)
        {
            Key = key;
            Values = values;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
