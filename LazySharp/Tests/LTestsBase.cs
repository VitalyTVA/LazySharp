using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LazySharp.Tests {
    public class LTestsBase {
        Track expectedTrack;
        Tracker tracker;
        [SetUp]
        public void SetUp() {
            tracker = new Tracker();
            expectedTrack = null;
        }
        public LTestsBase AddValueTrack<T>(T value) {
            expectedTrack = expectedTrack.AddValue(value);
            return this;
        }
        public LTestsBase AddFuncTrack<T>(Expression<Func<Func<T>>> func) {
            var info = GetFuncInfo(func);
            expectedTrack = expectedTrack.AddFunc(info.Item1, info.Item2);
            return this;
        }
        public LTestsBase AssertTracks() {
            Assert.AreEqual(expectedTrack, tracker.Track);
            return this;
        }

        public L<T> AsLazyTrackable<T>(T value) {
            return tracker.AsTrackable(value);
        }
        public L<T> MakeLazyTrackable<T>(Expression<Func<Func<T>>> func) {
            var info = GetFuncInfo(func);
            return tracker.MakeTrackable(info.Item1, info.Item2);
        }

        static Tuple<Func<T>, string> GetFuncInfo<T>(Expression<Func<Func<T>>> func) {
            MemberExpression body = (MemberExpression)func.Body;
            ConstantExpression constant = (ConstantExpression)body.Expression;
            return new Tuple<Func<T>, string>((Func<T>)((FieldInfo)body.Member).GetValue(constant.Value), body.Member.Name);
        }
    }
}
