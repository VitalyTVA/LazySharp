using System;
using LazySharp.Utils;

namespace LazySharp.Generated {
    public class List<T> {
        public static readonly List<T> Null = default(List<T>);
        public List(L<T> head, L<List<T>> tail) {
            head.NotNull();
            tail.NotNull();
            Head = head;
            Tail = tail;
        }
        public L<T> Head { get; private set; }
        public L<List<T>> Tail { get; private set; }
    }
}
