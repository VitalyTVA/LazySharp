using LazySharp.Generated;
using LazySharp.Utils;
using System;

namespace LazySharp {
    public static class LList {
        public static L<List<int>> Infinite(L<int> start) {
            start.NotNull();
            return new L<List<int>>(() => {
                return new List<int>(start, Infinite(new L<int>(() => start.Value() + 1)));
            });
        }
        public static L<List<int>> Range(L<int> start, L<int> count) {
            return Infinite(start).Take(count);
            //start.NotNull();
            //count.NotNull();
            //return new L<List<int>>(() => {
            //    if(count.GreaterThan(0.AsLazy()).Value())
            //        return new List<int>(start, Range(start.Inc(), count.Dec()));
            //    else
            //        return null;
            //});
        }
        public static L<List<T>> Take<T>(this L<List<T>> list, L<int> count) {
            list.NotNull();
            count.NotNull();
            return new L<List<T>>(() => {
                if(count.EqualTo(0.AsLazy()).Value())
                    return null;
                if(list.EqualTo(List<T>.Null).Value())
                    return null;
                return new List<T>(/*->>>> should be Lazy??*/ list.Value().Head, list.Value().Tail.Take(count.Dec()));
            });
        }
    }
}
