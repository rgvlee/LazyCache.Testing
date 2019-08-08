using LazyCache.Testing.Common.Tests;
using LazyCache.Testing.Moq.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System;

namespace LazyCache.Testing.Moq.Tests {
    [TestFixture]
    public class Tests : LazyCache.Testing.Common.Tests.Tests {
        protected Mock<IAppCache> cacheMock;
        
        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult) {
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            cacheMock = MockFactory.CreateCachingServiceMock();
            mockedCache = cacheMock.Object;
        }
        
        [Test]
        public void GetOrAddWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult);
                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Guid>>()), Times.Once);
            });
        }

        [Test]
        public void GetOrAddWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();
            
            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult);
                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, TestObject>>()), Times.Once);
            });
        }

        [Test]
        public void GetThenGetOrAddThenGetWithNoSetUp_TestObject_ReturnsExpectedResults() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = default(TestObject);
            var expectedResult2 = new TestObject();
            var expectedResult3 = expectedResult2;
            
            var actualResult1 = mockedCache.Get<TestObject>(cacheEntryKey);
            var actualResult2 = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult2, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = mockedCache.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.IsNull(actualResult1);
                Assert.AreEqual(expectedResult1, actualResult1);

                Assert.AreEqual(expectedResult2, actualResult2);

                Assert.IsNotNull(actualResult3);
                Assert.AreEqual(expectedResult3, actualResult3);

                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, TestObject>>()), Times.Once);
                cacheMock.Verify(m => m.Get<TestObject>(It.IsAny<string>()), Times.Exactly(2));
            });
        }

        [Test]
        public void GetThenGetOrAddThenGetWithSetUp_TestObject_ReturnsExpectedResults() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            SetUpCacheEntry(cacheEntryKey, expectedResult);
            
            var actualResult1 = mockedCache.Get<TestObject>(cacheEntryKey);
            var actualResult2 = mockedCache.GetOrAdd(cacheEntryKey, addItemFactory: () => expectedResult, expires: DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = mockedCache.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult1);
                Assert.AreEqual(expectedResult, actualResult2);
                Assert.AreEqual(expectedResult, actualResult3);

                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, TestObject>>()), Times.Once);
                cacheMock.Verify(m => m.Get<TestObject>(It.IsAny<string>()), Times.Exactly(2));
            });
        }
    }
}