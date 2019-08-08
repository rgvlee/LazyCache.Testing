using LazyCache.Testing.Moq.Extensions;
using Moq;
using NUnit.Framework;

namespace LazyCache.Testing.Moq.Tests {
    [TestFixture]
    public class Tests : LazyCache.Testing.Common.Tests.TestBase {
        protected Mock<IAppCache> CacheMock;
        
        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult) {
            CacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }
        
        [SetUp]
        public override void SetUp() {
            base.SetUp();

            CacheMock = MockFactory.CreateCachingServiceMock();
            MockedCache = CacheMock.Object;
        }
    }
}