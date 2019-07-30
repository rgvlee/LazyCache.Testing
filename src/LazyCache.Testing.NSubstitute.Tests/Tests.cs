using LazyCache.Testing.NSubstitute.Extensions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace LazyCache.Testing.NSubstitute.Tests {
    [TestFixture]
    public class Tests {
        [Test]
        public void GetOrAddWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
            
            var actualResult = cacheMock.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
            
            var actualResult = await cacheMock.GetOrAddAsync(cacheKey, () => Task.FromResult(expectedResult));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetWithNoSetUp_ReturnsNull() {
            var cacheKey = "SomethingInTheCache";

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = cacheMock.Get<string>(cacheKey);

            Assert.IsNull(actualResult);
        }

        [Test]
        public void GetOrAddWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();

            var actualResult = cacheMock.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult);
                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, string>>());
            });
        }

        [Test]
        public void GetOrAddWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = cacheMock.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult);
                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, TestObject>>());
            });
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = await cacheMock.GetOrAddAsync(cacheKey, () => Task.FromResult(expectedResult));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = await cacheMock.GetOrAddAsync(cacheKey, () => Task.FromResult(expectedResult));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetThenGetOrAddThenGetWithNoSetUp_TestObject_ReturnsExpectedResults() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult1 = default(TestObject);
            var expectedResult2 = new TestObject();
            var expectedResult3 = default(TestObject);

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult1 = cacheMock.Get<TestObject>(cacheKey);
            var actualResult2 = cacheMock.GetOrAdd(cacheKey, () => expectedResult2, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = cacheMock.Get<TestObject>(cacheKey);

            Assert.Multiple(() => {
                Assert.IsNull(actualResult1);
                Assert.AreSame(expectedResult1, actualResult1);

                Assert.AreSame(expectedResult2, actualResult2);

                Assert.IsNull(actualResult3);
                Assert.AreSame(expectedResult3, actualResult3);

                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, TestObject>>());
                cacheMock.Received(2).Get<TestObject>(Arg.Any<string>());
            });
        }

        [Test]
        public void GetThenGetOrAddThenGetWithSetUp_TestObject_ReturnsExpectedResults() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
            
            var actualResult1 = cacheMock.Get<TestObject>(cacheKey);
            var actualResult2 = cacheMock.GetOrAdd(cacheKey, addItemFactory: () => expectedResult, expires: DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = cacheMock.Get<TestObject>(cacheKey);

            Assert.Multiple(() => {
                Assert.AreSame(expectedResult, actualResult1);
                Assert.AreSame(expectedResult, actualResult2);
                Assert.AreSame(expectedResult, actualResult3);

                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, TestObject>>());
                cacheMock.Received(2).Get<TestObject>(Arg.Any<string>());
            });
        }

        [Test]
        public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = MockFactory.CreateCachingServiceMock();

            var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }
    }
}