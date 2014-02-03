using LazySharp.Utils;
using System;

namespace LazySharp {
    public static class LList {
        public static L<LList<int>> Infinite(L<int> start) {
            start.NotNull();
            return new L<LList<int>>(() => {
                return new LList<int>(start, Infinite(start.Inc()));
            });
        }
        public static L<LList<int>> Range(L<int> start, L<int> count) {
            return Infinite(start).Take(count);
            //start.NotNull();
            //count.NotNull();
            //return new L<LList<int>>(() => {
            //    if(count.GreaterThan(0.AsLazy()).Value())
            //        return new LList<int>(start, Range(start.Inc(), count.Dec()));
            //    else
            //        return null;
            //});
        }
        public static L<LList<T>> Take<T>(this L<LList<T>> list, L<int> count) {
            list.NotNull();
            count.NotNull();
            return new L<LList<T>>(() => {
                if(count.EqualTo(0.AsLazy()).Value())
                    return null;
                if(list.EqualTo(LList<T>.Null).Value())
                    return null;
                return new LList<T>(/*->>>> should be Lazy??*/ list.Value().Head, list.Value().Tail.Take(count.Dec()));
            });
        }
    }
}
