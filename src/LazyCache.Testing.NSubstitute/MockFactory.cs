using System;

namespace LazyCache.Testing.NSubstitute {
    /// <summary>
    ///     Factory for creating mock/mocked instances.
    /// </summary>
    [Obsolete("Use Create.MockedCachingService() to create a mocked caching service. This class will removed in a future release.")]
    public class MockFactory {
        /// <summary>
        ///     Creates a caching service mock.
        /// </summary>
        /// <returns>A caching service mock.</returns>
        public static IAppCache CreateCachingServiceMock() {
            return Create.MockedCachingService();
        }
    }
}