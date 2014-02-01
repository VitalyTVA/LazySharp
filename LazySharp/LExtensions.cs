using LazySharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp {
    public static class LExtensions {
        public static L<T> AsLazy<T>(this T value) {
            return new L<T>(value);
        }
        public static L<T> MakeLazy<T>(this Func<T> func) {
            return new L<T>(func);
        }
        public static L<int> Inc(this L<int> value) {
            return new L<int>(() => value.Value() + 1);
        }
        public static L<int> Dec(this L<int> value) {
            return new L<int>(() => value.Value() - 1);
        }
        //public static L<int> Dec(L<int> value) {
        //    return new L<int>(() => value.Value() - 1);
        //}
        public static IEnumerable<T> AsEnumerable<T>(this L<LList<T>> list) {
            list.NotNull();
            //while(list != null && list.Value() != null) {
            //    yield return list.Value().Head.Value();
            //    list = list.Value().Tail;
            //}
            do {
                var value = list.Value();
                if(value == null)
                    yield break;
                yield return value.Head.Value();
                list = value.Tail;
            } while(list != null);
        }
        //public static L<int> Add(this L<int> l1, L<int> l2) { 
        //    return new L<int>(() => l1.Value() + l2.Value());
        //}
    }
}
