using LazySharp.Utils;
using System;

namespace LazySharp {
    public class List<T> {
        public static readonly L<List<T>> Null = L<List<T>>.Default;
        public List(L<T> head, L<List<T>> tail) {
            Head = head.NotNull();
            Tail = tail.NotNull();
        }
        public L<T> Head { get; private set; }
        public L<List<T>> Tail { get; private set; }
    }
}
