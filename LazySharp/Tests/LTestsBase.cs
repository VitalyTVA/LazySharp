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
        protected void AddValueTrack<T>(T value) {
            track = track.AddValue(value);
        }
        protected internal void AddFuncTrack<T>(Func<T> func, string name) {
            track = track.AddFunc(func, name);
        }
        protected void AssertTracks() {
            Assert.AreEqual(track, DefaultTracker.Tracker.Track);
        }
    }
}
