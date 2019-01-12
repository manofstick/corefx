﻿using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Array<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        internal T[] Underlying { get; }

        public Array(T[] array, Link<T, V> first) : base(first) =>
            Underlying = array;

        public override Consumable<W> Create<W>(Link<T, W> first) =>
            new Array<T, W>(Underlying, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Array.Get(this);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Array.Invoke(Underlying, Link, consumer);
    }
}
