
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazySharp.Tests {
    public static class DefaultTracker {
        public static Tracker Tracker { get; private set; }
        public static void Reset() {
            Tracker = new Tracker();
        }
        public static L<T> AsLazyTrackable<T>(this T value) {
            return Tracker.AsTrackable(value);
        }
        public static L<T> MakeLazyTrackable<T>(this Func<T> func, string name = null) {
            return Tracker.MakeTrackable(func, name);
        }
    }
    public class Tracker {
        public Track Track { get; private set; }
        public Tracker() {
        }
        public L<T> AsTrackable<T>(T value) {
            return new L<T>(() => {
                Track = Track.AddValue(value);
                return value;
            });
        }
        public L<T> MakeTrackable<T>(Func<T> func, string name) {
            return new L<T>(() => {
                Track = Track.AddFunc(func, name);
                return func();
            });
        }
    }
    //TODO use [Lazy] LList instead;
    public class Track {
        readonly object trackItem;
        readonly Track next;
        public Track(object trackItem, Track next) {
            this.trackItem = trackItem;
            this.next = next;
        }
        public override bool Equals(object obj) {
            var other = obj as Track;
            return EqualsCore(this, obj as Track);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        public static bool EqualsCore(Track t1, Track t2) {
            if(t1 == null && t2 == null)
                return true;
            if(t1 != null && t2 != null)
                return object.Equals(t1.trackItem, t2.trackItem) && EqualsCore(t1.next, t2.next);
            return false;
        }
        public override string ToString() {
            return "Track: " + ToStringCore();
        }
        string ToStringCore() {
            return Convert.ToString(trackItem) + (next != null ? " -> " + next.ToStringCore() : string.Empty);
        }
    }
    public static class TrackExtensions {
        public static Track AddValue<T>(this Track track, T value) {
            return new Track(value, track);
        }
        public static Track AddFunc<T>(this Track track, Func<T> func, string name) {
            return new Track((object)name ?? func, track);
        }
    }
}
