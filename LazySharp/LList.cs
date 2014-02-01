using LazySharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp {
    public class LList<T> {
        public LList(L<T> head, L<LList<T>> tail = null) {
            Head = head.NotNull();
            Tail = tail;
        }
        public L<T> Head { get; private set; }
        public L<LList<T>> Tail { get; private set; }
    }

    public static class LList {
        public static L<LList<int>> Infinite(L<int> start) {
            return new L<LList<int>>(() => {
                return new LList<int>(start, Infinite(start.Inc()));
            });
        }
        public static L<LList<int>> Range(L<int> start, L<int> count) {
            return new L<LList<int>>(() => {
                return count.Value() > 0 ? 
                    new LList<int>(start, Range(start.Inc(), count.Dec())) : 
                    null;
            });
        }
    }
}
