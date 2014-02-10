using System;
using LazySharp.Utils;

namespace LazySharp.Prototypes {
    public static class List {
        public static List<int> Infinite(int start) {
            return null;
        }
        //public static List<int> Range(int start, int count) {
        //    return count > 0 ?
        //        new List<int>(start, Range(start + 1, count - 1)) :
        //        null;
        //}
        //public static List<T> Take<T>(this List<T> list, int count) {
        //    if(list == null)
        //        return null;
        //    if(count == 0)
        //        return null;
        //    return new List<T>(list.Head, list.Tail.Take(count - 1));
        //}
    }
}
