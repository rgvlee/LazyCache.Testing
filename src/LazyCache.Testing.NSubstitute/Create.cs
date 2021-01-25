using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using rgvlee.Core.Common.Helpers;

namespace LazyCache.Testing.NSubstitute
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
            var mock = Substitute.For<IAppCache>();

            var cacheDefaultsMock = Substitute.For<CacheDefaults>();
            mock.DefaultCachePolicy.Returns(cacheDefaultsMock);

            mock.When(x => x.Add(Arg.Any<string>(), Arg.Is<object>(y => y == null), Arg.Any<MemoryCacheEntryOptions>()))
                .Do(x =>
                {
                    Logger.LogDebug("Cache Add with null cache entry value invoked");
                    throw new ArgumentNullException("item");
                });

            var router = SubstitutionContext.Current.GetCallRouterFor(mock);
            router.RegisterCustomCallHandlerFactory(state => new NoSetUpHandler(mock));

            return mock;
        }
    }
}