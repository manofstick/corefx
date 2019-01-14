﻿using System.Collections;
using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed class Empty<T> : ConsumableForAddition<T>, IEnumerator<T>, IConsumableInternal
    {
        public static Consumable<T> Instance = new Empty<T>();

        private Empty() { }

        public Consumable<W> Create<W>(Link<T, W> first) => Empty<W>.Instance;

        public override Consumable<T> AddTail(Link<T, T> transform) => this;
        public override Consumable<U> AddTail<U>(Link<T, U> transform) => Empty<U>.Instance;

        public override IEnumerator<T> GetEnumerator() => this;

        public override void Consume(Chain<T> consumer)
        {
            try
            {
                consumer.ChainComplete();
            }
            finally
            {
                consumer.ChainDispose();
            }
        }

        void IDisposable.Dispose() { }
        bool IEnumerator.MoveNext() => false;
        void IEnumerator.Reset() { }
        object IEnumerator.Current => default;
        T IEnumerator<T>.Current => default;
    }
}
