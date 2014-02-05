using System;
using System.Linq;

namespace LazySharp.Utils {
    static class Argument {
        public static T NotNull<T>(this T argument) {
            if(argument == null)
                throw new ArgumentNullException();
            return argument;
        }
    }
}
