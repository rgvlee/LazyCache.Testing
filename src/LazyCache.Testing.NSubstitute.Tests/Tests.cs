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
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
            
            var actualResult = cacheMock.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
            
            var actualResult = await cacheMock.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetWithNoSetUp_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = cacheMock.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(default(Guid)));
        }

        [Test]
        public void GetOrAddWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();

            var actualResult = cacheMock.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult);
                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, Guid>>());
            });
        }

        [Test]
        public void GetOrAddWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = cacheMock.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult);
                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, TestObject>>());
            });
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = await cacheMock.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult = await cacheMock.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetThenGetOrAddThenGetWithNoSetUp_TestObject_ReturnsExpectedResults() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = default(TestObject);
            var expectedResult2 = new TestObject();
            var expectedResult3 = expectedResult2;

            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult1 = cacheMock.Get<TestObject>(cacheEntryKey);
            var actualResult2 = cacheMock.GetOrAdd(cacheEntryKey, () => expectedResult2, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = cacheMock.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.IsNull(actualResult1);
                Assert.AreEqual(expectedResult1, actualResult1);

                Assert.AreEqual(expectedResult2, actualResult2);

                Assert.IsNotNull(actualResult3);
                Assert.AreEqual(expectedResult3, actualResult3);

                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, TestObject>>());
                cacheMock.Received(2).Get<TestObject>(Arg.Any<string>());
            });
        }

        [Test]
        public void GetThenGetOrAddThenGetWithSetUp_TestObject_ReturnsExpectedResults() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
            
            var actualResult1 = cacheMock.Get<TestObject>(cacheEntryKey);
            var actualResult2 = cacheMock.GetOrAdd(cacheEntryKey, addItemFactory: () => expectedResult, expires: DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = cacheMock.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult1);
                Assert.AreEqual(expectedResult, actualResult2);
                Assert.AreEqual(expectedResult, actualResult3);

                cacheMock.Received(1).GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, TestObject>>());
                cacheMock.Received(2).Get<TestObject>(Arg.Any<string>());
            });
        }

        [Test]
        public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();

            var actualResult = cacheMock.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AddThenGetWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();

            cacheMock.Add(cacheEntryKey, expectedResult);
            var actualResult = cacheMock.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AddThenGetWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();
            var expectedResult2 = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult1);

            var actualResult1 = cacheMock.Get<Guid>(cacheEntryKey);
            
            cacheMock.Add(cacheEntryKey, expectedResult2);
            var actualResult2 = cacheMock.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult1, actualResult1);
                Assert.AreEqual(expectedResult2, actualResult2);
            });
        }

        [Test]
        public void RemoveWithSetUp_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();
            
            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult1);

            var actualResult1 = cacheMock.Get<Guid>(cacheEntryKey);
            cacheMock.Remove(cacheEntryKey);
            var actualResult2 = cacheMock.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult1, actualResult1);
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }

        [Test]
        public void RemoveWithNoSetUp_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            
            var cacheMock = MockFactory.CreateCachingServiceMock();
            
            var actualResult1 = cacheMock.Get<Guid>(cacheEntryKey);
            cacheMock.Remove(cacheEntryKey);
            var actualResult2 = cacheMock.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.That(actualResult1, Is.EqualTo(default(Guid)));
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }
    }
}