using System;
using Moq;

namespace LazyCache.Testing.Moq {
    /// <summary>
    ///     Factory for creating mock/mocked instances.
    /// </summary>
    [Obsolete("Use Create.MockedCachingService() to create a mocked caching service. This class will removed in a future release.")]
    public class MockFactory {
        /// <summary>
        ///     Creates a caching service mock.
        /// </summary>
        /// <returns>A caching service mock.</returns>
        public static Mock<IAppCache> CreateCachingServiceMock() {
            return Mock.Get(Create.MockedCachingService());
        }

        /// <summary>
        ///     Creates a mocked caching service.
        /// </summary>
        /// <returns>A mocked caching service.</returns>
        public static IAppCache CreateMockedCachingService() {
            return Create.MockedCachingService();
        }
    }
}