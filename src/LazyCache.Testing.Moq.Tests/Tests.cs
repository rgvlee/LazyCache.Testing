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

            MockedCache = Create.MockedCachingService();
        }

        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult)
        {
            MockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }

        [Test]
        public virtual void AddThenGetWithNoSetUp_TestObject_GetInvokedOnce()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            MockedCache.Add(cacheEntryKey, expectedResult);

            var actualResult = MockedCache.Get<TestObject>(cacheEntryKey);

            Mock.Get(MockedCache).Verify(m => m.Get<TestObject>(cacheEntryKey), Times.Once);
        }

        [Test]
        public virtual void AddWithNoSetUp_TestObject_AddInvokedOnce()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            MockedCache.Add(cacheEntryKey, expectedResult);

            Mock.Get(MockedCache).Verify(m => m.Add(cacheEntryKey, expectedResult, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }
    }
}