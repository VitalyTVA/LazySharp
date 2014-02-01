using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp.Tests {
    [TestFixture]
    public class LListTests {
        [Test]
        public void LList_Null() {
            var enumerable = ((LList<int>)null).AsLazy().AsEnumerable();
            Assert.Throws<ArgumentNullException>(() => enumerable.ToArray());

            enumerable = ((L<LList<int>>)null).AsEnumerable();
            Assert.Throws<ArgumentNullException>(() => enumerable.ToArray());
        }

        [Test]
        public void LList_HeadOnly() {
            int callCount = 0;
            Func<int> f1 = () => {
                callCount++;
                return 13;
            };
            var list = new LList<int>(f1.MakeLazy()).AsLazy();
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
            var list = new LList<int>(f1.MakeLazy(), new LList<int>(f2.MakeLazy()).AsLazy()).AsLazy();
            IEnumerator<int> en = list.AsEnumerable().GetEnumerator();
            callCount1.IsEqual(0);
            callCount2.IsEqual(0);
            en.MoveNext().IsTrue();
            callCount1.IsEqual(1);
            callCount2.IsEqual(0);
            en.Current.IsEqual(9);
            callCount1.IsEqual(1);
            callCount2.IsEqual(0);

            en.MoveNext().IsTrue();
            callCount1.IsEqual(1);
            callCount2.IsEqual(1);
            en.Current.IsEqual(13);
            callCount1.IsEqual(1);
            callCount2.IsEqual(1);

            en.MoveNext().IsFalse();
        }
    }
}
