using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp.Tests {
    [TestFixture]
    public class LListTests {
        [Test]
        public void Null() {
            var enumerable = ((L<LList<int>>)null).AsEnumerable();
            Assert.Throws<ArgumentNullException>(() => enumerable.ToArray());

            Assert.Throws<ArgumentNullException>(() => new LList<int>(0.AsLazy(), null));
        }
        [Test]
        public void Empty() {
            LList<int> empty = null;
            empty.AsLazy().AsEnumerable().Any().IsFalse();
        }
        [Test]
        public void HeadOnly() {
            int callCount = 0;
            Func<int> f1 = () => {
                callCount++;
                return 13;
            };
            var list = new LList<int>(f1.MakeLazy(), LList<int>.Null).AsLazy();
            list.AsEnumerable().Single().IsEqual(13);
            callCount.IsEqual(1);
            list.AsEnumerable().Single().IsEqual(13);
            callCount.IsEqual(1);
        }

        [Test]
        public void TwoElements() {
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

            int listCallCount2 = 0;
            Func<LList<int>> fList2 = () => {
                listCallCount2++;
                return new LList<int>(f2.MakeLazy(), LList<int>.Null);
            };

            int listCallCount1 = 0;
            Func<LList<int>> fList1 = () => {
                listCallCount1++;
                return new LList<int>(f1.MakeLazy(), fList2.MakeLazy());
            };

            var list = fList1.MakeLazy();
            IEnumerator<int> en = list.AsEnumerable().GetEnumerator();
            callCount1.IsEqual(0); callCount2.IsEqual(0); listCallCount1.IsEqual(0); listCallCount2.IsEqual(0);
            en.MoveNext().IsTrue();
            callCount1.IsEqual(1); listCallCount1.IsEqual(1); callCount2.IsEqual(0); listCallCount2.IsEqual(0);

            en.Current.IsEqual(9);
            callCount2.IsEqual(0); listCallCount2.IsEqual(0);

            en.MoveNext().IsTrue();
            callCount2.IsEqual(1); listCallCount2.IsEqual(1);

            en.Current.IsEqual(13);

            en.MoveNext().IsFalse();

            callCount1.IsEqual(1);
            callCount2.IsEqual(1);
            listCallCount1.IsEqual(1);
            listCallCount2.IsEqual(1);
        }

        [Test]
        public void Infinite() {
            int expected = 2;
            foreach(int number in LList.Infinite(expected.AsLazy()).AsEnumerable()) {
                number.IsEqual(expected);
                expected++;
                if(expected > 10)
                    break;
            }

            Assert.Throws<ArgumentNullException>(() => LList.Infinite(null));
        }
        [Test]
        public void Range() {
            int expected = 3;
            foreach(int number in LList.Range(expected.AsLazy(), 5.AsLazy()).AsEnumerable()) {
                number.IsEqual(expected);
                expected.IsTrue(x => x <= 3 + 5);
                expected++;
            }
            expected.IsEqual(3 + 5);

            LList.Range(5.AsLazy(), 1.AsLazy()).AsEnumerable().Single().IsEqual(5);
            LList.Range(5.AsLazy(), 0.AsLazy()).AsEnumerable().Any().IsFalse();

            Assert.Throws<ArgumentNullException>(() => LList.Range(null, L<int>.Default));
            Assert.Throws<ArgumentNullException>(() => LList.Range(L<int>.Default, null));
        }
    }
}
