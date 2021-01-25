using System;
using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Helpers;

namespace LazyCache.Testing.Moq
{
    /// <summary>
    ///     Factory for creating mocked instances.
    /// </summary>
    public static class Create
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(Create));

        /// <summary>
        ///     Creates a mocked caching service.
        /// </summary>
        /// <returns>A mocked caching service.</returns>
        public static IAppCache MockedCachingService()
        {
            var mockProxy = new ProxyGenerator().CreateClassProxy<Mock<IAppCache>>(new NoSetUpInterceptor());

            var cacheDefaultsMock = new Mock<CacheDefaults>();
            mockProxy.Setup(m => m.DefaultCachePolicy).Returns(cacheDefaultsMock.Object);

            mockProxy.DefaultValueProvider = new NoSetUpDefaultValueProvider(mockProxy.Object);

            mockProxy.Setup(m => m.Add(It.IsAny<string>(), It.Is<object>(x => x == null), It.IsAny<MemoryCacheEntryOptions>()))
                .Callback((string key, object item, MemoryCacheEntryOptions providedPolicy) =>
                {
                    Logger.LogDebug("Cache Add with null cache entry value invoked");
                    throw new ArgumentNullException("item");
                });

            return mockProxy.Object;
        }
    }
}