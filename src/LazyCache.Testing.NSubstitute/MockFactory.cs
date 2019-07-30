using NSubstitute;
using NSubstitute.Core;

namespace LazyCache.Testing.NSubstitute {
    /// <summary>
    /// Factory for creating mock/mocked instances.
    /// </summary>
    public class MockFactory {
        /// <summary>
        /// Creates a caching service mock.
        /// </summary>
        /// <returns>A caching service mock.</returns>
        public static IAppCache CreateCachingServiceMock() {
            var mock = Substitute.For<IAppCache>();

            var cacheDefaultsMock = Substitute.For<CacheDefaults>();
            mock.DefaultCachePolicy.Returns(cacheDefaultsMock);

            var router = SubstitutionContext.Current.GetCallRouterFor(mock);
            router.RegisterCustomCallHandlerFactory(state => new NoSetUpHandler());

            return mock;
        }
    }
}
