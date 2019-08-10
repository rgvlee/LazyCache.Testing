using LazyCache.Testing.Common.Extensions;
using LazyCache.Testing.Common.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LazyCache.Testing.NSubstitute.Extensions {
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
        public static IAppCache SetUpCacheEntry<T>(this IAppCache cachingServiceMock, string cacheEntryKey, T cacheEntryValue) {
            Logger.LogDebug($"Setting up cache entry for '{cacheEntryKey}' (type: {typeof(T).Name}; value: '{cacheEntryValue.ToString()}')");

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
        public static IAppCache SetUpCacheEntryAdd<T>(this IAppCache cachingServiceMock, string cacheEntryKey) {
            Logger.LogDebug($"Setting up cache entry Add for '{cacheEntryKey}'");

            cachingServiceMock
                .When(x => x.Add(Arg.Is<string>(s => s.Equals(cacheEntryKey)), Arg.Any<T>(), Arg.Any<MemoryCacheEntryOptions>()))
                .Do(x => {
                    Logger.LogDebug("Cache Add invoked");

                    //x provides the args as objects; this means we have to invoke the get set up dynamically to set the item type
                    //cachingServiceMock.SetUpCacheEntryGet(key, item);

                    var args = x.Args();
                    
                    var key = args[0].ToString();
                    var value = args[1];
                    var valueType = x.ArgTypes()[1];

                    var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntryGet"));
                    method.MakeGenericMethod(valueType).Invoke(null, new object[] { cachingServiceMock, key, value });
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
        public static IAppCache SetUpCacheEntryGet<T>(this IAppCache cachingServiceMock, string cacheEntryKey, T cacheEntryValue) {
            Logger.LogDebug($"Setting up cache entry Get/GetOrAdd for '{cacheEntryKey}' (type: {typeof(T).Name}; value: '{cacheEntryValue.ToString()}')");

            cachingServiceMock.Get<T>(Arg.Is<string>(s => s.Equals(cacheEntryKey)))
                .Returns(cacheEntryValue)
                .AndDoes(x => Logger.LogDebug("Cache Get invoked"));

            cachingServiceMock.GetOrAdd<T>(
                    Arg.Any<string>(),
                    Arg.Any<Func<ICacheEntry, T>>())
                .Returns(cacheEntryValue)
                .AndDoes(x => Logger.LogDebug("Cache GetOrAdd invoked"));

            cachingServiceMock.GetAsync<T>(
                    Arg.Is<string>(s => s.Equals(cacheEntryKey)))
                .Returns(Task.FromResult(cacheEntryValue))
                .AndDoes(x => Logger.LogDebug("Cache GetAsync invoked"));

            cachingServiceMock.GetOrAddAsync<T>(
                    Arg.Any<string>(),
                    Arg.Any<Func<ICacheEntry, Task<T>>>())
                .Returns(Task.FromResult(cacheEntryValue))
                .AndDoes(x => Logger.LogDebug("Cache GetOrAddAsync invoked"));

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
        public static IAppCache SetUpCacheEntryRemove<T>(this IAppCache cachingServiceMock, string cacheEntryKey) {
            Logger.LogDebug($"Setting up cache entry Remove for '{cacheEntryKey}'");

            cachingServiceMock
                .When(x => x.Remove(Arg.Is<string>(s => s.Equals(cacheEntryKey))))
                .Do(x => {
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
