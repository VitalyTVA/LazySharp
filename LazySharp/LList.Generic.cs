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
}
