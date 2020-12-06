using LazyCache.Testing.Common.Tests;
using LazyCache.Testing.Moq.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace LazyCache.Testing.Moq.Tests
{
    [TestFixture]
    public class Tests : TestBase
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

            Mock.Get(CachingService).Verify(m => m.Get<TestObject>(cacheEntryKey), Times.Once);
        }

        [Test]
        public virtual void AddWithNoSetUp_TestObject_AddInvokedOnce()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            CachingService.Add(cacheEntryKey, expectedResult);

            Mock.Get(CachingService).Verify(m => m.Add(cacheEntryKey, expectedResult, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }
    }
}