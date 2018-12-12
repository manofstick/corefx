using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    internal class ConsumableEnumerable<T, V> : ConsumableWithComposition<T, V>
    {
        private readonly IEnumerable<T> enumerable;

        public ConsumableEnumerable(IEnumerable<T> enumerable, ILink<T, V> first) : base(first) =>
            this.enumerable = enumerable;

        public override Consumable<W> Create<W>(ILink<T, W> first) =>
            new ConsumableEnumerable<T, W>(enumerable, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Enumerable.Get(enumerable, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Enumerable.Invoke(enumerable, Link, consumer);
    }
}
