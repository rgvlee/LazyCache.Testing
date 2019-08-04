using Castle.DynamicProxy;
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
            var mockProxy = new ProxyGenerator().CreateClassProxy<Mock<IAppCache>>(new NoSetUpInterceptor());
            
            var cacheDefaultsMock = new Mock<CacheDefaults>();
            mockProxy.Setup(m => m.DefaultCachePolicy).Returns(cacheDefaultsMock.Object);

            mockProxy.DefaultValueProvider = new NoSetUpDefaultValueProvider(mockProxy);

            return mockProxy;
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