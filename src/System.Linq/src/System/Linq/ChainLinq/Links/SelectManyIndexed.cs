﻿using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    internal class SelectManyIndexed<T, U> : ILink<T, (T, IEnumerable<U>)>
    {
        private readonly Func<T, int, IEnumerable<U>> collectionSelector;

        public SelectManyIndexed(Func<T, int, IEnumerable<U>> collectionSelector) =>
            this.collectionSelector = collectionSelector;

        public Chain<T, V> Compose<V>(Chain<(T, IEnumerable<U>), V> next) =>
            new Activity<V>(next, collectionSelector);

        private class Activity<V> : Activity<T, (T, IEnumerable<U>), V>
        {
            private readonly Func<T, int, IEnumerable<U>> collectionSelector;
            private int index = 0;

            public Activity(Chain<(T, IEnumerable<U>)> next, Func<T, int, IEnumerable<U>> collectionSelector) : base(next) =>
                this.collectionSelector = collectionSelector;

            public override ChainStatus ProcessNext(T input) =>
                Next((input, collectionSelector(input, index++)));
        }
    }
}
