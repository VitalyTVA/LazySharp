using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp.Tests {
    public class LTestsBase {
        Track track;
        [SetUp]
        public void SetUp() {
            DefaultTracker.Reset();
            track = null;
        }
        public LTestsBase AddValueTrack<T>(T value) {
            track = track.AddValue(value);
            return this;
        }
        public LTestsBase AddFuncTrack<T>(Func<T> func, string name = null) {
            track = track.AddFunc(func, name);
            return this;
        }
        public LTestsBase AssertTracks() {
            Assert.AreEqual(track, DefaultTracker.Tracker.Track);
            return this;
        }
    }
}
