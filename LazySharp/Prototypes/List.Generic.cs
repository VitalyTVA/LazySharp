using System;
using LazySharp.Utils;

namespace LazySharp.Prototypes {
    public class List<T> {
        public List(T head, List<T> tail) {
            Head = head;
            Tail = tail;
        }
        public T Head { get; private set; }
        public List<T> Tail { get; private set; }
    }
}
