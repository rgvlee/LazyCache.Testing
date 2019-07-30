using LazyCache.Testing.Moq.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace LazyCache.Testing.Moq.Tests {
    [TestFixture]
    public class Tests {
        [Test]
        public void GetOrAddWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
            var mockedCache = cacheMock.Object;

            var actualResult = await mockedCache.GetOrAddAsync(cacheKey, () => Task.FromResult(expectedResult));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetWithNoSetUp_ReturnsNull() {
            var cacheKey = "SomethingInTheCache";

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.Get<string>(cacheKey);

            Assert.IsNull(actualResult);
        }

        [Test]
        public void GetOrAddWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult);
                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, string>>()), Times.Once);
            });
        }

        [Test]
        public void GetOrAddWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult);
                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, TestObject>>()), Times.Once);
            });
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = await mockedCache.GetOrAddAsync(cacheKey, () => Task.FromResult(expectedResult));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = await mockedCache.GetOrAddAsync(cacheKey, () => Task.FromResult(expectedResult));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetThenGetOrAddThenGetWithNoSetUp_TestObject_ReturnsExpectedResults() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult1 = default(TestObject);
            var expectedResult2 = new TestObject();
            var expectedResult3 = default(TestObject);

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult1 = mockedCache.Get<TestObject>(cacheKey);
            var actualResult2 = mockedCache.GetOrAdd(cacheKey, () => expectedResult2, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = mockedCache.Get<TestObject>(cacheKey);

            Assert.Multiple(() => {
                Assert.IsNull(actualResult1);
                Assert.AreSame(expectedResult1, actualResult1);

                Assert.AreSame(expectedResult2, actualResult2);

                Assert.IsNull(actualResult3);
                Assert.AreSame(expectedResult3, actualResult3);

                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, TestObject>>()), Times.Once);
                cacheMock.Verify(m => m.Get<TestObject>(It.IsAny<string>()), Times.Exactly(2));
            });
        }

        [Test]
        public void GetThenGetOrAddThenGetWithSetUp_TestObject_ReturnsExpectedResults() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
            var mockedCache = cacheMock.Object;
            
            var actualResult1 = mockedCache.Get<TestObject>(cacheKey);
            var actualResult2 = mockedCache.GetOrAdd(cacheKey, addItemFactory: () => expectedResult, expires: DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = mockedCache.Get<TestObject>(cacheKey);

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult1);
                Assert.AreSame(expectedResult, actualResult2);
                Assert.AreSame(expectedResult, actualResult3);

                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, TestObject>>()), Times.Once);
                cacheMock.Verify(m => m.Get<TestObject>(It.IsAny<string>()), Times.Exactly(2));
            });
        }

        [Test]
        public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = MockFactory.CreateMockedCachingService();
            
            var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }
    }
}