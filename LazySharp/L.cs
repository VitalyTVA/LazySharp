using System;
using LazySharp.Utils;

namespace LazySharp {
    public class L<T> {
        public static readonly L<T> Default = new L<T>(default(T));
        T value;
        Func<T> func;

        public T Value() {
            if(!HasValue) {
                value = func();
                func = null;
            }
            return value;
        }
        public bool HasValue { get { return func == null; } }

        public L(T value) {
            this.value = value;
        }
        public L(Func<T> func) {
            this.func = func.NotNull();
        }
    }
}
