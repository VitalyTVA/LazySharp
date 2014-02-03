using System;
namespace LazySharp.Generated {
    public class List<T> {
        public List(T head, List<T> tail) {
            Head = head;
            Tail = tail;
        }
        public T Head { get; private set; }
        public List<T> Tail { get; private set; }
    }
}