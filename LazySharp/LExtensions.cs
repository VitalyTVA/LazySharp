using LazySharp.Generated;
using LazySharp.Utils;
using System;
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
        public static L<bool> GreaterThan(this L<int> left, L<int> right) {
            return new L<bool>(() => left.Value() > right.Value());
        }
        public static L<bool> EqualTo(this L<int> left, L<int> right) {
            return new L<bool>(() => left.Value() == right.Value());
        }
        public static L<bool> EqualTo<T>(this L<T> left, L<T> right) {
            return new L<bool>(() => object.Equals(left.Value(), right.Value()));
        }

        public static System.Collections.Generic.IEnumerable<T> AsEnumerable<T>(this L<List<T>> list) {
            list.NotNull();
            List<T> listValue = null;
            while((listValue = list.Value()) != null) {
                yield return listValue.Head.Value();
                list = listValue.Tail;
            }
        }
        //public static L<int> Add(this L<int> l1, L<int> l2) { 
        //    return new L<int>(() => l1.Value() + l2.Value());
        //}
    }
}
