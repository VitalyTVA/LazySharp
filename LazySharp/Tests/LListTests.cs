using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp.Tests {
    [TestFixture]
    public class LListTests : LTestsBase {
        [Test]
        public void Null() {
            var enumerable = ((L<LList<int>>)null).AsEnumerable();
            Assert.Throws<ArgumentNullException>(() => enumerable.ToArray());

            Assert.Throws<ArgumentNullException>(() => new LList<int>(0.AsLazy(), null));
        }
        [Test]
        public void SingleNull() {
            new LList<object>(L<object>.Default, LList<object>.Null).AsLazy().AsEnumerable().Single().IsNull();
        }
        [Test]
        public void Empty() {
            LList<int> empty = null;
            empty.AsLazy().AsEnumerable().Any().IsFalse();
        }
        [Test]
        public void HeadOnly() {
            var list = new LList<int>(AsLazyTrackable(13), LList<int>.Null).AsLazy();
            base.AssertTracks();
            list.AsEnumerable().Single().IsEqual(13);
            base.AddValueTrack(13).AssertTracks();
            list.AsEnumerable().Single().IsEqual(13);
            base.AssertTracks();
        }

        [Test]
        public void TwoElements() {
            Func<LList<int>> fList2 = () => new LList<int>(AsLazyTrackable(13), LList<int>.Null);
            Func<LList<int>> fList1 = () => new LList<int>(AsLazyTrackable(9), MakeLazyTrackable(fList2, "fList2"));

            var list = MakeLazyTrackable(fList1, "fList1");
            IEnumerator<int> en = list.AsEnumerable().GetEnumerator();
            base.AssertTracks();
            en.MoveNext().IsTrue();
            base.AddFuncTrack(fList1, "fList1").AddValueTrack(9).AssertTracks();

            en.Current.IsEqual(9);
            base.AssertTracks();

            en.MoveNext().IsTrue();
            base.AddFuncTrack(fList2, "fList2").AddValueTrack(13).AssertTracks();

            en.Current.IsEqual(13);
            base.AssertTracks();

            en.MoveNext().IsFalse();
            base.AssertTracks();
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
        public void Infinite_LazyStartEvaluation() {
            var infinite = LList.Infinite(AsLazyTrackable(7));
            base.AssertTracks();
            infinite = infinite.Value().Tail;
            base.AssertTracks();
            infinite.Value();
            base.AssertTracks();
            infinite.Value().Head.Value().IsEqual(8);
            base.AddValueTrack(7).AssertTracks();
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
        [Test, Explicit]
        public void Weird() {
            var weird = GetWeird();
            var x = weird.Value();
        }
        L<int> GetWeird() { 
            return new L<int>(() => GetWeird().Value() + 1);
        }
    }
}
