using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Extensions;
using rgvlee.Core.Common.Helpers;
using ProjectReflectionShortcuts = LazyCache.Testing.NSubstitute.Helpers.ReflectionShortcuts;

namespace LazyCache.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for mocks.
    /// </summary>
    public static class MockExtensions
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(MockExtensions));

        internal static IAppCache SetUpCacheEntry<T>(this IAppCache mockedCachingService, string cacheEntryKey, T cacheEntryValue)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug("Setting up cache entry for '{cacheEntryKey}' (type: '{type}'; value: '{cacheEntryValue}')", cacheEntryKey, typeof(T), cacheEntryValue);

            mockedCachingService.SetUpCacheEntryAdd<T>(cacheEntryKey);

            mockedCachingService.SetUpCacheEntryGet(cacheEntryKey, cacheEntryValue);

            mockedCachingService.SetUpCacheEntryRemove<T>(cacheEntryKey);

            return mockedCachingService;
        }

        internal static IAppCache SetUpCacheEntryAdd<T>(this IAppCache mockedCachingService, string cacheEntryKey)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug("Setting up cache entry Add for '{cacheEntryKey}'", cacheEntryKey);

            mockedCachingService.When(x => x.Add(Arg.Is<string>(s => s.Equals(cacheEntryKey)), Arg.Any<T>(), Arg.Any<MemoryCacheEntryOptions>()))
                .Do(x =>
                {
                    Logger.LogDebug("Cache Add invoked");

                    //x provides the args as objects; this means we have to use reflection to set the item type for the SetUpCacheEntryGet method
                    var args = x.Args();

                    var key = args[0].ToString();
                    var value = args[1];

                    ProjectReflectionShortcuts.SetUpCacheEntryGetMethod(value.GetType()).Invoke(null, new[] { mockedCachingService, key, value });
                });

            return mockedCachingService;
        }

        internal static IAppCache SetUpCacheEntryGet<T>(this IAppCache mockedCachingService, string cacheEntryKey, T cacheEntryValue)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug("Setting up cache entry Get/GetOrAdd for '{cacheEntryKey}' (type: '{type}'; value: '{cacheEntryValue}')", cacheEntryKey, typeof(T), cacheEntryValue);

            mockedCachingService.Get<T>(Arg.Is<string>(s => s.Equals(cacheEntryKey))).Returns(cacheEntryValue).AndDoes(x => Logger.LogDebug("Cache Get invoked"));

            mockedCachingService.GetOrAdd(Arg.Is<string>(s => s.Equals(cacheEntryKey)), Arg.Any<Func<ICacheEntry, T>>())
                .Returns(cacheEntryValue)
                .AndDoes(x => Logger.LogDebug("Cache GetOrAdd invoked"));

            //Backwards compatibility
            if (ProjectReflectionShortcuts.GetOrAddWithMemoryCacheEntryOptionsMethod != null)
            {
                Logger.LogDebug("Setting up GetOrAddWithMemoryCacheEntryOptionsMethod");

                ProjectReflectionShortcuts.GetOrAddWithMemoryCacheEntryOptionsMethod.MakeGenericMethod(typeof(T))
                    .Invoke(mockedCachingService.Configure(),
                        new object[] { Arg.Is<string>(s => s.Equals(cacheEntryKey)), Arg.Any<Func<ICacheEntry, T>>(), Arg.Any<MemoryCacheEntryOptions>() })
                    .Returns(cacheEntryValue)
                    .AndDoes(x => Logger.LogDebug("Cache GetOrAdd invoked"));
            }

            mockedCachingService.GetAsync<T>(Arg.Is<string>(s => s.Equals(cacheEntryKey)))
                .Returns(Task.FromResult(cacheEntryValue))
                .AndDoes(x => Logger.LogDebug("Cache GetAsync invoked"));

            mockedCachingService.GetOrAddAsync(Arg.Is<string>(s => s.Equals(cacheEntryKey)), Arg.Any<Func<ICacheEntry, Task<T>>>())
                .Returns(Task.FromResult(cacheEntryValue))
                .AndDoes(x => Logger.LogDebug("Cache GetOrAddAsync invoked"));

            //Backwards compatibility
            if (ProjectReflectionShortcuts.GetOrAddAsyncWithMemoryCacheEntryOptionsMethod != null)
            {
                Logger.LogDebug("Setting up GetOrAddAsyncWithMemoryCacheEntryOptionsMethod");

                ProjectReflectionShortcuts.GetOrAddAsyncWithMemoryCacheEntryOptionsMethod.MakeGenericMethod(typeof(T))
                    .Invoke(mockedCachingService.Configure(),
                        new object[] { Arg.Is<string>(s => s.Equals(cacheEntryKey)), Arg.Any<Func<ICacheEntry, Task<T>>>(), Arg.Any<MemoryCacheEntryOptions>() })
                    .Returns(Task.FromResult(cacheEntryValue))
                    .AndDoes(x => Logger.LogDebug("Cache GetOrAddAsync invoked"));
            }

            return mockedCachingService;
        }

        internal static IAppCache SetUpCacheEntryRemove<T>(this IAppCache mockedCachingService, string cacheEntryKey)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug("Setting up cache entry Remove for '{cacheEntryKey}'", cacheEntryKey);

            mockedCachingService.When(x => x.Remove(Arg.Is<string>(s => s.Equals(cacheEntryKey))))
                .Do(x =>
                {
                    Logger.LogDebug("Cache Remove invoked");
                    ProjectReflectionShortcuts.SetUpCacheEntryGetMethod(typeof(T)).Invoke(null, new object[] { mockedCachingService, cacheEntryKey, default(T) });
                });

            return mockedCachingService;
        }
    }
}