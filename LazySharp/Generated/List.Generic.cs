using System;
using LazySharp.Utils;

namespace LazySharp.Generated {
    public class List<T> {
        public static readonly L<List<T>> Null = new L<List<T>>(default(List<T>));

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
