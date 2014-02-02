using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public LTestsBase AddFuncTrack<T>(Func<T> func, string name = null) {
            expectedTrack = expectedTrack.AddFunc(func, name);
            return this;
        }
        public LTestsBase AssertTracks() {
            Assert.AreEqual(expectedTrack, tracker.Track);
            return this;
        }

        public L<T> AsLazyTrackable<T>(T value) {
            return tracker.AsTrackable(value);
        }
        public L<T> MakeLazyTrackable<T>(Func<T> func, string name = null) {
            return tracker.MakeTrackable(func, name);
        }
    }
}
