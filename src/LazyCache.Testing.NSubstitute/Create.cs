using NSubstitute;
using NSubstitute.Core;

namespace LazyCache.Testing.NSubstitute {
    /// <summary>
    /// Factory for creating mocked instances.
    /// </summary>
    public class Create {
        /// <summary>
        ///     Creates a mocked caching service.
        /// </summary>
        /// <returns>A mocked caching service.</returns>
        public static IAppCache MockedCachingService() {
            var mock = Substitute.For<IAppCache>();

            var cacheDefaultsMock = Substitute.For<CacheDefaults>();
            mock.DefaultCachePolicy.Returns(cacheDefaultsMock);

            var router = SubstitutionContext.Current.GetCallRouterFor(mock);
            router.RegisterCustomCallHandlerFactory(state => new NoSetUpHandler(mock));

            return mock;
        }
    }
}