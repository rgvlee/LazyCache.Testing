using LazyCache.Testing.NSubstitute.Extensions;
using NUnit.Framework;

namespace LazyCache.Testing.NSubstitute.Tests {
    [TestFixture]
    public class Tests : LazyCache.Testing.Common.Tests.TestBase {
        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult) {
            MockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }
        
        [SetUp]
        public override void SetUp() {
            base.SetUp();

            MockedCache = MockFactory.CreateCachingServiceMock();
        }
    }
}