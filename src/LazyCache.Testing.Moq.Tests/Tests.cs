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
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));
            
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
            var mockedCache = cacheMock.Object;

            var actualResult = await mockedCache.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetWithNoSetUp_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(default(Guid)));
        }

        [Test]
        public void GetOrAddWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

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

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult);
                cacheMock.Verify(m => m.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, TestObject>>()), Times.Once);
            });
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = await mockedCache.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = await mockedCache.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetThenGetOrAddThenGetWithNoSetUp_TestObject_ReturnsExpectedResults() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = default(TestObject);
            var expectedResult2 = new TestObject();
            var expectedResult3 = expectedResult2;

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

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

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
            var mockedCache = cacheMock.Object;
            
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

        [Test]
        public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var mockedCache = MockFactory.CreateMockedCachingService();
            
            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        //The ability to handle an unexpected void method is not supported by Moq4
        //so I am using a dynamic proxy on the mock to ensure that add methods are set up automatically
        [Test]
        public void AddThenGetWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            Console.WriteLine("Add invocation started");

            mockedCache.Add(cacheEntryKey, expectedResult);

            Console.WriteLine("Add invocation finished");

            var actualResult = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AddThenGetWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            mockedCache.Add(cacheEntryKey, expectedResult);

            var actualResult = mockedCache.Get<TestObject>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AddThenGetWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();
            var expectedResult2 = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult1);
            var mockedCache = cacheMock.Object;

            var actualResult1 = mockedCache.Get<Guid>(cacheEntryKey);
            Assert.AreEqual(expectedResult1, actualResult1);

            mockedCache.Add(cacheEntryKey, expectedResult2);
            var actualResult2 = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult2, actualResult2);
        }

        [Test]
        public void RemoveWithSetUp_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult1);
            var mockedCache = cacheMock.Object;

            var actualResult1 = mockedCache.Get<Guid>(cacheEntryKey);
            mockedCache.Remove(cacheEntryKey);
            var actualResult2 = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult1, actualResult1);
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }

        [Test]
        public void RemoveWithNoSetUp_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            
            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult1 = mockedCache.Get<Guid>(cacheEntryKey);
            mockedCache.Remove(cacheEntryKey);
            var actualResult2 = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.That(actualResult1, Is.EqualTo(default(Guid)));
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }
    }
}