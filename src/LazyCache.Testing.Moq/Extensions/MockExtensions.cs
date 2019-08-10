using LazyCache.Testing.Common.Extensions;
using LazyCache.Testing.Common.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LazyCache.Testing.Moq.Extensions {
    /// <summary>
    /// Extensions for mocks.
    /// </summary>
    public static class MockExtensions {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(MockExtensions));

        /// <summary>
        /// Sets up a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="cachingServiceMock">The caching service mock instance.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <param name="cacheEntryValue">The cache entry value.</param>
        /// <returns>The caching service mock.</returns>
        public static Mock<IAppCache> SetUpCacheEntry<T>(this Mock<IAppCache> cachingServiceMock, string cacheEntryKey, T cacheEntryValue) {
            Logger.LogDebug($"Setting up cache entry for '{cacheEntryKey}' (type: {typeof(T).Name}; value: '{cacheEntryValue}')");

            cachingServiceMock.SetUpCacheEntryAdd<T>(cacheEntryKey);

            cachingServiceMock.SetUpCacheEntryGet(cacheEntryKey, cacheEntryValue);

            cachingServiceMock.SetUpCacheEntryRemove<T>(cacheEntryKey);

            return cachingServiceMock;
        }

        /// <summary>
        /// Sets up the add method for a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="cachingServiceMock">The caching service mock instance.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <returns>The caching service mock.</returns>
        /// <remarks>I've left this accessible for advanced usage. In most cases you should just use <see cref="SetUpCacheEntry{T}"/>.</remarks>
        public static Mock<IAppCache> SetUpCacheEntryAdd<T>(this Mock<IAppCache> cachingServiceMock, string cacheEntryKey) {
            Logger.LogDebug($"Setting up cache entry Add for '{cacheEntryKey}'");

            cachingServiceMock.Setup(m => m.Add<T>(
                    It.Is<string>(s => s.Equals(cacheEntryKey)),
                    It.IsAny<T>(),
                    It.IsAny<MemoryCacheEntryOptions>()))
                .Callback((string key, T item, MemoryCacheEntryOptions policyParameter) => {
                    Logger.LogDebug("Cache Add invoked");
                    cachingServiceMock.SetUpCacheEntryGet(key, item);
                });

            return cachingServiceMock;
        }

        /// <summary>
        /// Sets up the get methods for a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="cachingServiceMock">The caching service mock instance.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <param name="cacheEntryValue">The cache entry value.</param>
        /// <returns>The caching service mock.</returns>
        /// <remarks>I've left this accessible for advanced usage. In most cases you should just use <see cref="SetUpCacheEntry{T}"/>.</remarks>
        public static Mock<IAppCache> SetUpCacheEntryGet<T>(this Mock<IAppCache> cachingServiceMock, string cacheEntryKey, T cacheEntryValue) {
            Logger.LogDebug($"Setting up cache entry Get/GetOrAdd for '{cacheEntryKey}' (type: {typeof(T).Name}; value: '{cacheEntryValue}')");

            cachingServiceMock.Setup(m => m.Get<T>(
                    It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() => Logger.LogDebug("Cache Get invoked"))
                .Returns(cacheEntryValue);

            cachingServiceMock.Setup(m => m.GetOrAdd<T>(
                    It.IsAny<string>(),
                    It.IsAny<Func<ICacheEntry, T>>()))
                .Callback(() => Logger.LogDebug("Cache GetOrAdd invoked"))
                .Returns(cacheEntryValue);

            cachingServiceMock.Setup(m => m.GetAsync<T>(
                    It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() => Logger.LogDebug("Cache GetAsync invoked"))
                .Returns(Task.FromResult(cacheEntryValue));

            cachingServiceMock.Setup(m => m.GetOrAddAsync<T>(
                    It.IsAny<string>(),
                    It.IsAny<Func<ICacheEntry, Task<T>>>()))
                .Callback(() => Logger.LogDebug("Cache GetOrAddAsync invoked"))
                .Returns(Task.FromResult(cacheEntryValue));
            
            return cachingServiceMock;
        }

        /// <summary>
        /// Sets up the remove method for a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="cachingServiceMock">The caching service mock instance.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <returns>The caching service mock.</returns>
        /// <remarks>I've left this accessible for advanced usage. In most cases you should just use <see cref="SetUpCacheEntry{T}"/>.</remarks>
        public static Mock<IAppCache> SetUpCacheEntryRemove<T>(this Mock<IAppCache> cachingServiceMock, string cacheEntryKey) {
            Logger.LogDebug($"Setting up cache entry Remove for '{cacheEntryKey}'");

            cachingServiceMock.Setup(m => m.Remove(
                    It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() => {
                    Logger.LogDebug("Cache Remove invoked");

                    var key = cacheEntryKey;
                    var value = typeof(T).GetDefaultValue();
                    var valueType = typeof(T);

                    var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntryGet"));
                    method.MakeGenericMethod(valueType).Invoke(null, new object[] { cachingServiceMock, key, value });
                });

            return cachingServiceMock;
        }
    }
}
