using System;

namespace LazySharp.Generated {
    public class List<T> {
        public List(L<T> head, L<List<T>> tail) {
            Head = head;
            Tail = tail;
        }
        public L<T> Head { get; private set; }
        public L<List<T>> Tail { get; private set; }
    }
}
