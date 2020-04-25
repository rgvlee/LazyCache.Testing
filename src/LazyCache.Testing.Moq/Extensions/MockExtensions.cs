using System;
using System.Linq;
using System.Threading.Tasks;
using LazyCache.Testing.Common;
using LazyCache.Testing.Common.Extensions;
using LazyCache.Testing.Common.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace LazyCache.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for mocks.
    /// </summary>
    public static class MockExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(MockExtensions));

        /// <summary>
        ///     Sets up a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="mockedCachingService">The mocked caching service.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <param name="cacheEntryValue">The cache entry value.</param>
        /// <returns>The mocked caching service.</returns>
        public static IAppCache SetUpCacheEntry<T>(this IAppCache mockedCachingService, string cacheEntryKey, T cacheEntryValue)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug($"Setting up cache entry for '{cacheEntryKey}' (type: {typeof(T).Name}; value: '{cacheEntryValue}')");

            mockedCachingService.SetUpCacheEntryAdd<T>(cacheEntryKey);

            mockedCachingService.SetUpCacheEntryGet(cacheEntryKey, cacheEntryValue);

            mockedCachingService.SetUpCacheEntryRemove<T>(cacheEntryKey);

            return mockedCachingService;
        }

        /// <summary>
        ///     Sets up the add method for a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="mockedCachingService">The mocked caching service.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <returns>The mocked caching service.</returns>
        /// <remarks>
        ///     I've left this accessible for advanced usage. In most cases you should just use
        ///     <see cref="SetUpCacheEntry{T}" />.
        /// </remarks>
        public static IAppCache SetUpCacheEntryAdd<T>(this IAppCache mockedCachingService, string cacheEntryKey)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug($"Setting up cache entry Add for '{cacheEntryKey}'");

            Mock.Get(mockedCachingService)
                .Setup(m => m.Add(It.Is<string>(s => s.Equals(cacheEntryKey)), It.IsAny<T>(), It.IsAny<MemoryCacheEntryOptions>()))
                .Callback((string key, T item, MemoryCacheEntryOptions policyParameter) =>
                {
                    Logger.LogDebug("Cache Add invoked");
                    mockedCachingService.SetUpCacheEntryGet(key, item);
                });

            return mockedCachingService;
        }

        /// <summary>
        ///     Sets up the get methods for a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="mockedCachingService">The mocked caching service.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <param name="cacheEntryValue">The cache entry value.</param>
        /// <returns>The mocked caching service.</returns>
        /// <remarks>
        ///     I've left this accessible for advanced usage. In most cases you should just use
        ///     <see cref="SetUpCacheEntry{T}" />.
        /// </remarks>
        public static IAppCache SetUpCacheEntryGet<T>(this IAppCache mockedCachingService, string cacheEntryKey, T cacheEntryValue)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug($"Setting up cache entry Get/GetOrAdd for '{cacheEntryKey}' (type: {typeof(T).Name}; value: '{cacheEntryValue}')");

            var cachingServiceMock = Mock.Get(mockedCachingService);

            cachingServiceMock
                .Setup(m => m.Get<T>(It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() => Logger.LogDebug("Cache Get invoked"))
                .Returns(cacheEntryValue);

            cachingServiceMock
                .Setup(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, T>>()))
                .Callback(() => Logger.LogDebug("Cache GetOrAdd invoked"))
                .Returns(cacheEntryValue);

            cachingServiceMock
                .Setup(m => m.GetAsync<T>(It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() => Logger.LogDebug("Cache GetAsync invoked"))
                .Returns(Task.FromResult(cacheEntryValue));

            cachingServiceMock
                .Setup(m => m.GetOrAddAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<T>>>()))
                .Callback(() => Logger.LogDebug("Cache GetOrAddAsync invoked"))
                .Returns(Task.FromResult(cacheEntryValue));

            return mockedCachingService;
        }

        /// <summary>
        ///     Sets up the remove method for a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="mockedCachingService">The mocked caching service.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <returns>The mocked caching service.</returns>
        /// <remarks>
        ///     I've left this accessible for advanced usage. In most cases you should just use
        ///     <see cref="SetUpCacheEntry{T}" />.
        /// </remarks>
        public static IAppCache SetUpCacheEntryRemove<T>(this IAppCache mockedCachingService, string cacheEntryKey)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug($"Setting up cache entry Remove for '{cacheEntryKey}'");

            Mock.Get(mockedCachingService)
                .Setup(m => m.Remove(It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() =>
                {
                    Logger.LogDebug("Cache Remove invoked");

                    var key = cacheEntryKey;
                    var value = typeof(T).GetDefaultValue();
                    var valueType = typeof(T);

                    var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntryGet"));
                    method.MakeGenericMethod(valueType).Invoke(null, new[] { mockedCachingService, key, value });
                });

            return mockedCachingService;
        }
    }
}