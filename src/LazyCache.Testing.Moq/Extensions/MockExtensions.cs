using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Helpers;
using ProjectReflectionShortcuts = LazyCache.Testing.Moq.Helpers.ReflectionShortcuts;
using CoreReflectionShortcuts = rgvlee.Core.Common.Helpers.ReflectionShortcuts;

namespace LazyCache.Testing.Moq.Extensions
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

            Logger.LogDebug("Setting up cache entry for '{cacheEntryKey}' (type: {type}; value: '{cacheEntryValue}')", cacheEntryKey, typeof(T), cacheEntryValue);

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

            Mock.Get(mockedCachingService)
                .Setup(m => m.Add(It.Is<string>(s => s.Equals(cacheEntryKey)), It.IsAny<T>(), It.IsAny<MemoryCacheEntryOptions>()))
                .Callback((string key, T item, MemoryCacheEntryOptions providedPolicy) =>
                {
                    Logger.LogDebug("Cache Add invoked");
                    mockedCachingService.SetUpCacheEntryGet(key, item);
                });

            return mockedCachingService;
        }

        internal static IAppCache SetUpCacheEntryGet<T>(this IAppCache mockedCachingService, string cacheEntryKey, T cacheEntryValue)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug("Setting up cache entry Get/GetOrAdd for '{cacheEntryKey}' (type: {type}; value: '{cacheEntryValue}')", cacheEntryKey, typeof(T), cacheEntryValue);

            var cachingServiceMock = Mock.Get(mockedCachingService);

            cachingServiceMock.Setup(m => m.Get<T>(It.Is<string>(s => s.Equals(cacheEntryKey)))).Callback(() => Logger.LogDebug("Cache Get invoked")).Returns(cacheEntryValue);

            cachingServiceMock.Setup(m => m.GetOrAdd(It.Is<string>(s => s.Equals(cacheEntryKey)), It.IsAny<Func<ICacheEntry, T>>()))
                .Callback(() => Logger.LogDebug("Cache GetOrAdd invoked"))
                .Returns(cacheEntryValue);

            //Backwards compatibility
            if (ProjectReflectionShortcuts.GetOrAddWithMemoryCacheEntryOptionsMethod != null)
            {
                Logger.LogDebug("Setting up GetOrAddWithMemoryCacheEntryOptionsMethod");

                var getOrAddExpression = ExpressionHelper.CreateMethodCallExpression<IAppCache, T>(
                    ProjectReflectionShortcuts.GetOrAddWithMemoryCacheEntryOptionsMethod.MakeGenericMethod(typeof(T)),
                    Expression.Call(ProjectReflectionShortcuts.ItIsMethod(typeof(string)),
                        ExpressionHelper.CreateMethodCallExpression<string, bool>(CoreReflectionShortcuts.StringEqualsMethodWithStringArgument,
                            Expression.Constant(cacheEntryKey, typeof(string)))),
                    Expression.Call(ProjectReflectionShortcuts.ItIsAnyMethod(typeof(Func<ICacheEntry, T>))),
                    Expression.Call(ProjectReflectionShortcuts.ItIsAnyMethod(typeof(MemoryCacheEntryOptions))));

                cachingServiceMock.Setup(getOrAddExpression).Callback(() => Logger.LogDebug("Cache GetOrAdd invoked")).Returns(cacheEntryValue);
            }

            cachingServiceMock.Setup(m => m.GetAsync<T>(It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() => Logger.LogDebug("Cache GetAsync invoked"))
                .Returns(Task.FromResult(cacheEntryValue));

            cachingServiceMock.Setup(m => m.GetOrAddAsync(It.Is<string>(s => s.Equals(cacheEntryKey)), It.IsAny<Func<ICacheEntry, Task<T>>>()))
                .Callback(() => Logger.LogDebug("Cache GetOrAddAsync invoked"))
                .Returns(Task.FromResult(cacheEntryValue));

            //Backwards compatibility
            if (ProjectReflectionShortcuts.GetOrAddAsyncWithMemoryCacheEntryOptionsMethod != null)
            {
                Logger.LogDebug("Setting up GetOrAddAsyncWithMemoryCacheEntryOptionsMethod");

                var getOrAddAsyncExpression = ExpressionHelper.CreateMethodCallExpression<IAppCache, Task<T>>(
                    ProjectReflectionShortcuts.GetOrAddWithMemoryCacheEntryOptionsMethod.MakeGenericMethod(typeof(T)),
                    Expression.Call(ProjectReflectionShortcuts.ItIsMethod(typeof(string)),
                        ExpressionHelper.CreateMethodCallExpression<string, bool>(CoreReflectionShortcuts.StringEqualsMethodWithStringArgument,
                            Expression.Constant(cacheEntryKey, typeof(string)))),
                    Expression.Call(ProjectReflectionShortcuts.ItIsAnyMethod(typeof(Func<ICacheEntry, Task<T>>))),
                    Expression.Call(ProjectReflectionShortcuts.ItIsAnyMethod(typeof(MemoryCacheEntryOptions))));

                cachingServiceMock.Setup(getOrAddAsyncExpression).Callback(() => Logger.LogDebug("Cache GetOrAddAsync invoked")).Returns(Task.FromResult(cacheEntryValue));
            }

            return mockedCachingService;
        }

        internal static IAppCache SetUpCacheEntryRemove<T>(this IAppCache mockedCachingService, string cacheEntryKey)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));
            EnsureArgument.IsNotNullOrEmpty(cacheEntryKey, nameof(cacheEntryKey));

            Logger.LogDebug("Setting up cache entry Remove for '{cacheEntryKey}'", cacheEntryKey);

            Mock.Get(mockedCachingService)
                .Setup(m => m.Remove(It.Is<string>(s => s.Equals(cacheEntryKey))))
                .Callback(() =>
                {
                    Logger.LogDebug("Cache Remove invoked");
                    ProjectReflectionShortcuts.SetUpCacheEntryGetMethod(typeof(T)).Invoke(null, new object[] { mockedCachingService, cacheEntryKey, default(T) });
                });

            return mockedCachingService;
        }
    }
}