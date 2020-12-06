using System.Reflection;
using LazyCache.Testing.Common.Tests;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;

namespace LazyCache.DefaultBehaviour.Tests
{
    public class Tests : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CachingService = new CachingService();
        }

        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult)
        {
            CachingService.Add(cacheEntryKey, expectedResult);
        }

        [TearDown]
        public virtual void TearDown()
        {
            var cacheProvider = CachingService.CacheProvider;
            var memoryCache = (MemoryCache)cacheProvider.GetType().GetField("cache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cacheProvider);
            memoryCache.Compact(1.0);
        }
    }
}