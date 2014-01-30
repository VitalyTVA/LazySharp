using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazySharp {
    public struct L<T> {
        T value;
        Func<T> func;

        public T Value { 
            get {
                if(!HasValue) {
                    value = func();
                    func = null;
                }
                return value;
            } 
        }
        public bool HasValue { get { return func != null; } }

        public L(T value) {
            this.value = value;
            this.func = null;
        }
        public L(Func<T> func) {
            this.value = default(T);
            this.func = null;
        }
    }
}
