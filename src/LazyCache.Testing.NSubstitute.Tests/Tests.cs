using LazyCache.Testing.Common.Tests;
using LazyCache.Testing.NSubstitute.Extensions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NUnit.Framework;

namespace LazyCache.Testing.NSubstitute.Tests
{
    public class Tests : BaseForTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CachingService = Create.MockedCachingService();
        }

        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult)
        {
            CachingService.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }

        [Test]
        public virtual void AddThenGetWithNoSetUp_TestObject_GetInvokedOnce()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            CachingService.Add(cacheEntryKey, expectedResult);

            var actualResult = CachingService.Get<TestObject>(cacheEntryKey);

            CachingService.Received(1).Get<TestObject>(cacheEntryKey);
        }

        [Test]
        public virtual void AddWithNoSetUp_TestObject_AddInvokedOnce()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            CachingService.Add(cacheEntryKey, expectedResult);

            CachingService.Received(1).Add(cacheEntryKey, expectedResult, Arg.Any<MemoryCacheEntryOptions>());
        }
    }
}