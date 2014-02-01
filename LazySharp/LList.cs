using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp {
    public class LList<T> {
        public LList(L<T> head, L<LList<T>> tail) {
            Tail = tail;
            Head = head;
        }
        public L<T> Head { get; private set; }
        public L<LList<T>> Tail { get; private set; }
    }
}
