﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (count <= 0)
            {
                // Return source if not actually skipping, but only if it's a type from here, to avoid
                // issues if collections are used as keys or otherwise must not be aliased.
                if (source is ChainLinq.Consumables.IConsumableInternal)
                {
                    return source;
                }

                count = 0;
            }

            var consumable = ChainLinq.Utils.AsConsumable(source);

            if (consumable is ChainLinq.Optimizations.ISkipTakeOnConsumable<TSource> opt)
            {
                return opt.Skip(count);
            }

            if (consumable is ChainLinq.ConsumableForMerging<TSource> merger)
            {
                if (merger.TailLink is ChainLinq.Optimizations.IMergeSkip<TSource> skipMerge)
                {
                    return skipMerge.MergeSkip(merger, count);
                }

                return merger.AddTail(CreateSkipLink<TSource>(count));
            }

            return ChainLinq.Utils.PushTransform(consumable, CreateSkipLink<TSource>(count));
        }

        private static ChainLinq.Links.Skip<TSource> CreateSkipLink<TSource>(int count) =>
            new ChainLinq.Links.Skip<TSource>(count);

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            return SkipWhileIterator(source, predicate);
        }

        private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    TSource element = e.Current;
                    if (!predicate(element))
                    {
                        yield return element;
                        while (e.MoveNext())
                        {
                            yield return e.Current;
                        }

                        yield break;
                    }
                }
            }
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            return SkipWhileIterator(source, predicate);
        }

        private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                int index = -1;
                while (e.MoveNext())
                {
                    checked
                    {
                        index++;
                    }

                    TSource element = e.Current;
                    if (!predicate(element, index))
                    {
                        yield return element;
                        while (e.MoveNext())
                        {
                            yield return e.Current;
                        }

                        yield break;
                    }
                }
            }
        }

        public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (count <= 0)
            {
                return source.Skip(0);
            }

            return SkipLastIterator(source, count);
        }

        private static IEnumerable<TSource> SkipLastIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            Debug.Assert(source != null);
            Debug.Assert(count > 0);

            var queue = new Queue<TSource>();

            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (queue.Count == count)
                    {
                        do
                        {
                            yield return queue.Dequeue();
                            queue.Enqueue(e.Current);
                        }
                        while (e.MoveNext());
                        break;
                    }
                    else
                    {
                        queue.Enqueue(e.Current);
                    }
                }
            }
        }
    }
}
