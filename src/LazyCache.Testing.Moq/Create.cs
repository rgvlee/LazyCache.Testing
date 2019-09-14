using Castle.DynamicProxy;
using Moq;

namespace LazyCache.Testing.Moq {
    /// <summary>
    ///     Factory for creating mock/mocked instances.
    /// </summary>
    public class Create {
        /// <summary>
        ///     Creates a mocked caching service.
        /// </summary>
        /// <returns>A mocked caching service.</returns>
        public static IAppCache MockedCachingService() {
            var mockProxy = new ProxyGenerator().CreateClassProxy<Mock<IAppCache>>(new NoSetUpInterceptor());

            var cacheDefaultsMock = new Mock<CacheDefaults>();
            mockProxy.Setup(m => m.DefaultCachePolicy).Returns(cacheDefaultsMock.Object);

            mockProxy.DefaultValueProvider = new NoSetUpDefaultValueProvider(mockProxy.Object);

            return mockProxy.Object;
        }
    }
}