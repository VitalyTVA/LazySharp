using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazySharp.Tests {
    [System.Diagnostics.DebuggerNonUserCode]
    public static class FluentAssert {
        public static TInput IsNull<TInput>(this TInput obj, Func<TInput, object> valueEvaluator = null) where TInput : class {
            Assert.IsNull(GetActualValue(obj, valueEvaluator));
            return obj;
        }
        public static TInput IsNotNull<TInput>(this TInput obj, Func<TInput, object> valueEvaluator = null) where TInput : class {
            Assert.IsNotNull(GetActualValue(obj, valueEvaluator));
            return obj;
        }
        public static TInput IsEqual<TInput>(this TInput obj, object expectedValue) {
            Assert.AreEqual(expectedValue, obj);
            return obj;
        }
        public static TInput IsEqual<TInput>(this TInput obj, Func<TInput, object> valueEvaluator, object expectedValue) {
            Assert.AreEqual(expectedValue, valueEvaluator(obj));
            return obj;
        }
        public static TInput IsNotEqual<TInput>(this TInput obj, object expectedValue) {
            Assert.AreNotEqual(expectedValue, obj);
            return obj;
        }
        public static TInput IsNotEqual<TInput>(this TInput obj, Func<TInput, object> valueEvaluator, object expectedValue) {
            Assert.AreNotEqual(expectedValue, valueEvaluator(obj));
            return obj;
        }
        public static TInput IsTrue<TInput>(this TInput obj, Func<TInput, bool> valueEvaluator) {
            Assert.IsTrue(valueEvaluator(obj));
            return obj;
        }
        public static TInput IsFalse<TInput>(this TInput obj, Func<TInput, bool> valueEvaluator) {
            Assert.IsFalse(valueEvaluator(obj));
            return obj;
        }
        public static bool IsTrue(this bool val) {
            Assert.IsTrue(val);
            return val;
        }
        public static bool IsFalse(this bool val) {
            Assert.IsFalse(val);
            return val;
        }
        public static TInput IsInstanceOfType<TInput>(this TInput obj, Type expectedType) where TInput : class {
            Assert.IsInstanceOfType(expectedType, obj);
            return obj;
        }
        public static TInput IsInstanceOfType<TInput>(this TInput obj, Func<TInput, object> valueEvaluator, Type expectedType) where TInput : class {
            Assert.IsInstanceOfType(expectedType, valueEvaluator(obj));
            return obj;
        }
        static object GetActualValue<TInput>(TInput obj, Func<TInput, object> valueEvaluator) {
            return valueEvaluator == null ? obj : valueEvaluator(obj);
        }
    }
}
