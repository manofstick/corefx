﻿using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.GetEnumerator
{
    // The compiler generated IEnumerator does not clear IEnumerator.Current which means
    // using these causes test failures.
    //
    // The could be manually created, which would be more efficient as a bonus, but I just
    // haven't got around to it.
#if COMPILER_GENERATED_ENUMERATORS_FOLLOWED_RULES_REQUIRED_BY_THE_TEST_SUITE
    static partial class Array
    {
        static partial void Optimized<T, U>(T[] array, ILink<T, U> link, ref IEnumerator<U> enumerator)
        { 
            switch (link)
            {
                case Links.Select<T, U> select:
                    enumerator = Select(array, select.Selector);
                    break;

                case Links.Where<T> where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = (IEnumerator<U>)Where(array, where.Predicate);
                    break;
            }
        }

        private static IEnumerator<T> Where<T>(T[] array, Func<T, bool> predicate)
        {
            foreach (var item in array)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        private static IEnumerator<U> Select<T, U>(T[] array, Func<T, U> selector)
        {
            foreach (var item in array)
            {
                yield return selector(item);
            }
        }
    }
#endif
}
