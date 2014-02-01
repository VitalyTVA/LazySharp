using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazySharp.Tests {
    [TestFixture]
    public class LTests {
        [Test]
        public void L_NullFunc() {
            Assert.Throws<ArgumentNullException>(() => ((Func<int>)null).MakeLazy());
        }

        [Test]
        public void L() {
            const int x = 9;
            x.AsLazy().HasValue.IsTrue();
            x.AsLazy().Value().IsEqual(9);

            int callCount = 0;
            Func<int> f = () => { 
                callCount++; 
                return 13; 
            };
            f.AsLazy().HasValue.IsTrue();
            callCount.IsEqual(0);
            f.AsLazy().Value().IsEqual(f);
            callCount.IsEqual(0);

            var lazy = f.MakeLazy();
            callCount.IsEqual(0);
            lazy.HasValue.IsFalse();
            callCount.IsEqual(0);
            lazy.Value().IsEqual(13);
            lazy.HasValue.IsTrue();
            lazy.Value().IsEqual(13);
            callCount.IsEqual(1);
        }

        [Test]
        public void LList_Null() {
            LList<int> list = null;
            list.AsEnumerable().Any().IsFalse();
        }

        [Test]
        public void LList_HeadOnly() {
            LList<int> list = null;
            list.AsEnumerable().Any().IsFalse();

            //Func<int> f1 = () => { 
            //    return 1; 
            //};
            //Func<LList<int>> f2 = () => { 
            //    throw new NotImplementedException();
            //};
            //var list = new LList<int>(f1.MakeLazy(), f2.MakeLazy());
        }
    }
}