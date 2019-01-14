﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Array
    {
        static partial void Optimized<T, U>(Consumables.Array<T, U> consumable, ref IEnumerator<U> enumerator)
        {
            switch (consumable.Link.LinkType)
            {
                case Links.LinkType.Select:
                    enumerator = new SelectEnumerator<T, U>(consumable.Underlying, ((Links.Select<T, U>)consumable.Link).Selector);
                    break;

                case Links.LinkType.Where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = GetWhereEnumerator<U>(consumable);
                    break;
            }
        }

        private static IEnumerator<U> GetWhereEnumerator<U>(object consumable)
        {
            var c = (Consumables.Array < U, U >) consumable;
            return new WhereEnumerator<U>(c.Underlying, ((Links.Where<U>)c.Link).Predicate);
        }

        abstract class EnumeratorBase<T> : IEnumerator<T>
        {
            object IEnumerator.Current => Current;
            public virtual void Dispose() { }
            public virtual void Reset() => throw Error.NotSupported();

            public T Current { get; protected set; }

            public abstract bool MoveNext();
        }

        sealed class WhereEnumerator<T> : EnumeratorBase<T>
        {
            private readonly T[] _array;
            private readonly Func<T, bool> _predicate;

            private int _idx;

            public WhereEnumerator(T[] array, Func<T, bool> predicate) =>
                (_array, _predicate) = (array, predicate);

            public override bool MoveNext()
            {
                while (_idx < _array.Length)
                {
                    var item = _array[_idx++];
                    if (_predicate(item))
                    {
                        Current = item;
                        return true;
                    }
                }

                _idx = _array.Length;
                Current = default;
                return false;
            }
        }

        sealed class SelectEnumerator<T, U> : EnumeratorBase<U>
        {
            private T[] _array;
            private Func<T, U> _selector;

            int _idx;

            public SelectEnumerator(T[] array, Func<T, U> selector) =>
                (_array, _selector) = (array, selector);

            public override bool MoveNext()
            {
                if (_idx >= _array.Length)
                {
                    Current = default;
                    return false;
                }

                Current = _selector(_array[_idx++]);
                return true;
            }
        }
    }
}
