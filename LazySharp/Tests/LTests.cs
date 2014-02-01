using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazySharp.Tests {
    [TestFixture]
    public class LTests {
        [Test]
        public void LazyTest() {
            int x = 9;
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
    }
}