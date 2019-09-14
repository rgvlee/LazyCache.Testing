using LazyCache.Testing.Common.Tests;
using LazyCache.Testing.Moq.Extensions;
using NUnit.Framework;

namespace LazyCache.Testing.Moq.Tests {
    [TestFixture]
    public class Tests : TestBase {
        [SetUp]
        public override void SetUp() {
            base.SetUp();

            MockedCache = Create.MockedCachingService();
        }

        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult) {
            MockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }
    }
}