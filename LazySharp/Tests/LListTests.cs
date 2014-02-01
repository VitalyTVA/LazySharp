using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp.Tests {
    [TestFixture]
    public class LListTests {
        [Test]
        public void LList_Null() {
            var enumerable = ((LList<int>)null).AsEnumerable();
            Assert.Throws<ArgumentNullException>(() => enumerable.ToArray());
        }

        [Test]
        public void LList_HeadOnly() {
            int callCount = 0;
            Func<int> f1 = () => {
                callCount++;
                return 13;
            };
            var list = new LList<int>(f1.MakeLazy());
            list.AsEnumerable().Single().IsEqual(13);
            callCount.IsEqual(1);
            list.AsEnumerable().Single().IsEqual(13);
            callCount.IsEqual(1);
        }

        [Test]
        public void LList_TwoElements() {
            int callCount1 = 0;
            Func<int> f1 = () => {
                callCount1++;
                return 9;
            };
            int callCount2 = 0;
            Func<int> f2 = () => {
                callCount2++;
                return 13;
            };
            var list = new LList<int>(f1.MakeLazy(), null);
            list.AsEnumerable().Single().IsEqual(13);
            callCount1.IsEqual(1);
            list.AsEnumerable().Single().IsEqual(13);
            callCount1.IsEqual(1);
        }
    }
}
