using LazySharp.Utils;
using System;

namespace LazySharp {
    public class LList<T> {
        public static readonly L<LList<T>> Null = L<LList<T>>.Default;
        public LList(L<T> head, L<LList<T>> tail) {
            Head = head.NotNull();
            Tail = tail.NotNull();
        }
        public L<T> Head { get; private set; }
        public L<LList<T>> Tail { get; private set; }
    }
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
                return new LList<T>(list.Value().Head, list.Value().Tail.Take(count.Dec()));
            });
        }
    }
    public class _LList<T> {
        public _LList(T head, _LList<T> tail) {
            Head = head;
            Tail = tail;
        }
        public T Head { get; private set; }
        public _LList<T> Tail { get; private set; }
    }

    public static class _LList {
        public static _LList<int> Infinite(int start) {
            return new _LList<int>(start, Infinite(start + 1));
        }
        public static _LList<int> Range(int start, int count) {
                return count > 0 ?
                    new _LList<int>(start, Range(start + 1, count - 1)) :
                    null;
        }
        public static _LList<T> Take<T>(this _LList<T> list, int count) {
            if(list == null)
                return null;
            if(count == 0)
                return null;
            return new _LList<T>(list.Head, list.Tail.Take(count - 1));
        }
    }   
}
