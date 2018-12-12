using System.Collections;
using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    internal abstract class ConsumableWithComposition<T, U> : Consumable<U>
    {
        protected ILink<T, U> Link { get; }

        protected ConsumableWithComposition(ILink<T, U> link) =>
            Link = link;

        public override Consumable<V> AddTail<V>(ILink<U, V> next) =>
            Create(Links.Composition.Create(Link, next));

        public abstract Consumable<V> Create<V>(ILink<T, V> first);
    }
}
