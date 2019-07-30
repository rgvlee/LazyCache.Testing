using Moq;

namespace LazyCache.Testing.Moq {
    /// <summary>
    /// Factory for creating mock/mocked instances.
    /// </summary>
    public class MockFactory {
        /// <summary>
        /// Creates a caching service mock.
        /// </summary>
        /// <returns>A caching service mock.</returns>
        public static Mock<IAppCache> CreateCachingServiceMock() {
            var mock = new Mock<IAppCache>();

            var cacheDefaultsMock = new Mock<CacheDefaults>();
            mock.Setup(m => m.DefaultCachePolicy).Returns(cacheDefaultsMock.Object);

            mock.DefaultValueProvider = new NoSetUpDefaultValueProvider();

            return mock;
        }

        /// <summary>
        /// Creates a mocked caching service.
        /// </summary>
        /// <returns>A mocked caching service.</returns>
        public static IAppCache CreateMockedCachingService() {
            return CreateCachingServiceMock().Object;
        }
    }
}