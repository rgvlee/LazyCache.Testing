using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;

namespace LazyCache.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for mocks.
    /// </summary>
    public static class MockExtensions
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(MockExtensions));

        /// <summary>
        ///     Sets up a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="mockedCachingService">The mocked caching service.</param>
        /// <param name="cacheEntryKey">The cache entry key.</param>
        /// <param name="cacheEntryValue">The cache entry value.</param>
        /// <returns>The mocked caching service.</returns>
        [Obsolete("Access to this method will be removed in a future version. Use the mocked caching service to maintain cache entries.")]
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
        [Obsolete("Access to this method will be removed in a future version. Use the mocked caching service to maintain cache entries.")]
        public static IAppCache SetUpCacheEntryAdd<T>(this IAppCache mockedCachingService, string cacheEntryKey)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug($"Setting up cache entry Add for '{cacheEntryKey}'");

            mockedCachingService.When(x => x.Add(Arg.Is<string>(s => s.Equals(cacheEntryKey)), Arg.Any<T>(), Arg.Any<MemoryCacheEntryOptions>()))
                .Do(x =>
                {
                    Logger.LogDebug("Cache Add invoked");

                    //x provides the args as objects; this means we have to invoke the get set up dynamically to set the item type
                    var args = x.Args();

                    var key = args[0].ToString();
                    var value = args[1];
                    var valueType = x.ArgTypes()[1];

                    var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntryGet"));
                    method.MakeGenericMethod(valueType).Invoke(null, new[] { mockedCachingService, key, value });
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
        [Obsolete("Access to this method will be removed in a future version. Use the mocked caching service to maintain cache entries.")]
        public static IAppCache SetUpCacheEntryGet<T>(this IAppCache mockedCachingService, string cacheEntryKey, T cacheEntryValue)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug($"Setting up cache entry Get/GetOrAdd for '{cacheEntryKey}' (type: {typeof(T).Name}; value: '{cacheEntryValue}')");

            mockedCachingService.Get<T>(Arg.Is<string>(s => s.Equals(cacheEntryKey))).Returns(cacheEntryValue).AndDoes(x => Logger.LogDebug("Cache Get invoked"));

            mockedCachingService.GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, T>>()).Returns(cacheEntryValue).AndDoes(x => Logger.LogDebug("Cache GetOrAdd invoked"));

            mockedCachingService.GetAsync<T>(Arg.Is<string>(s => s.Equals(cacheEntryKey)))
                .Returns(Task.FromResult(cacheEntryValue))
                .AndDoes(x => Logger.LogDebug("Cache GetAsync invoked"));

            mockedCachingService.GetOrAddAsync(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, Task<T>>>())
                .Returns(Task.FromResult(cacheEntryValue))
                .AndDoes(x => Logger.LogDebug("Cache GetOrAddAsync invoked"));

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
        [Obsolete("Access to this method will be removed in a future version. Use the mocked caching service to maintain cache entries.")]
        public static IAppCache SetUpCacheEntryRemove<T>(this IAppCache mockedCachingService, string cacheEntryKey)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug($"Setting up cache entry Remove for '{cacheEntryKey}'");

            mockedCachingService.When(x => x.Remove(Arg.Is<string>(s => s.Equals(cacheEntryKey))))
                .Do(x =>
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