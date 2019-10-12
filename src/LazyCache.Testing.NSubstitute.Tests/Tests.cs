using LazyCache.Testing.Common.Tests;
using LazyCache.Testing.NSubstitute.Extensions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NUnit.Framework;

namespace LazyCache.Testing.NSubstitute.Tests {
    [TestFixture]
    public class Tests : TestBase {
        [SetUp]
        public override void SetUp() {
            base.SetUp();

            MockedCache = Create.MockedCachingService();
        }

        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult) {
            MockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }

        [Test]
        public virtual void AddWithNoSetUp_TestObject_AddInvokedOnce() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            MockedCache.Add(cacheEntryKey, expectedResult);

            MockedCache.Received(1).Add(cacheEntryKey, expectedResult, Arg.Any<MemoryCacheEntryOptions>());
        }

        [Test]
        public virtual void AddThenGetWithNoSetUp_TestObject_GetInvokedOnce() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            MockedCache.Add(cacheEntryKey, expectedResult);

            var actualResult = MockedCache.Get<TestObject>(cacheEntryKey);

            MockedCache.Received(1).Get<TestObject>(cacheEntryKey);
        }
    }
}