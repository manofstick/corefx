﻿namespace System.Linq.ChainLinq.Consumer
{
    // TODO: Remove?
    class SetResult<T> : Consumer<T, T>
    {
        public SetResult() : base(default) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result = input;
            return ChainStatus.Flow;
        }
    }
}
