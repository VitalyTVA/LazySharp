using LazySharp.Generated;
using NUnit.Framework;
using System;
using System.Linq;

namespace LazySharp.Tests {
    [TestFixture]
    public class LListTests : LTestsBase {
        [Test]
        public void Null() {
            var enumerable = ((L<List<int>>)null).AsEnumerable();
            Assert.Throws<ArgumentNullException>(() => enumerable.ToArray());

            Assert.Throws<ArgumentNullException>(() => new List<int>(0.AsLazy(), null));
        }
        [Test]
        public void SingleNull() {
            new List<object>(L<object>.Default, List<object>.Null).AsLazy().AsEnumerable().Single().IsNull();
        }
        [Test]
        public void Empty() {
            List<int> empty = null;
            empty.AsLazy().AsEnumerable().Any().IsFalse();
        }
        [Test]
        public void HeadOnly() {
            var list = new List<int>(AsLazyTrackable(13), List<int>.Null).AsLazy();
            base.AssertTracks();
            list.AsEnumerable().Single().IsEqual(13);
            base.AddValueTrack(13).AssertTracks();
            list.AsEnumerable().Single().IsEqual(13);
            base.AssertTracks();
        }

        [Test]
        public void TwoElements() {
            Func<List<int>> fList2 = () => new List<int>(AsLazyTrackable(13), List<int>.Null);
            Func<List<int>> fList1 = () => new List<int>(AsLazyTrackable(9), MakeLazyTrackable(() => fList2));

            var list = MakeLazyTrackable(() => fList1);
            var en = list.AsEnumerable().GetEnumerator();
            base.AssertTracks();
            en.MoveNext().IsTrue();
            base.AddFuncTrack(() => fList1).AddValueTrack(9).AssertTracks();

            en.Current.IsEqual(9);
            base.AssertTracks();

            en.MoveNext().IsTrue();
            base.AddFuncTrack(() => fList2).AddValueTrack(13).AssertTracks();

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
        [Test]
        public void Range_LazyEvaluation() {
            var range = LList.Range(AsLazyTrackable(5), AsLazyTrackable(2));
            base.AssertTracks();
            range = range.Value().Tail;
            base.AddValueTrack(2).AssertTracks();
            range.Value();
            base.AssertTracks();
            range.Value().Head.Value().IsEqual(6);
            base.AddValueTrack(5).AssertTracks();
        }
        [Test]
        public void Take_NullArgs() {
            Assert.Throws<ArgumentNullException>(() => LList.Take((L<List<int>>)null, L<int>.Default));
            Assert.Throws<ArgumentNullException>(() => LList.Take(List<int>.Null, null));
        }
        [Test]
        public void Take_LazyEvaluation() {
            Func<List<string>> fList3 = () => new List<string>(AsLazyTrackable("3"), List<string>.Null);
            Func<List<string>> fList2 = () => new List<string>(AsLazyTrackable("2"), MakeLazyTrackable(() => fList3));
            Func<List<string>> fList1 = () => new List<string>(AsLazyTrackable("1"), MakeLazyTrackable(() => fList2));

            var first = MakeLazyTrackable(() => fList1).Take(AsLazyTrackable(2));
            base.AssertTracks();

            var second = first.Value().Tail;
            base.AddValueTrack(2).AddFuncTrack(() => fList1).AssertTracks();

            var third = second.Value().Tail;
            base.AddFuncTrack(() => fList2).AssertTracks();

            third.Value().IsNull();
            base.AssertTracks();

            second.Value().Head.Value().IsEqual("2");
            base.AddValueTrack("2").AssertTracks();

            first.Value().Head.Value().IsEqual("1");
            base.AddValueTrack("1").AssertTracks();
        }
        [Test]
        public void Take_LazyEvaluation_ShortList() {
            Func<List<string>> fList2 = () => new List<string>(AsLazyTrackable("2"), List<string>.Null);
            Func<List<string>> fList1 = () => new List<string>(AsLazyTrackable("1"), MakeLazyTrackable(() => fList2));

            var first = MakeLazyTrackable(() => fList1).Take(AsLazyTrackable(3));
            base.AssertTracks();

            var second = first.Value().Tail;
            base.AddValueTrack(3).AddFuncTrack(() => fList1).AssertTracks();

            var third = second.Value().Tail;
            base.AddFuncTrack(() => fList2).AssertTracks();

            third.Value().IsNull();
            base.AssertTracks();
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
