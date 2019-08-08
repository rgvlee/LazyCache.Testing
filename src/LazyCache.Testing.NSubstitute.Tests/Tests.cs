using LazyCache.Testing.Common.Tests;
using LazyCache.Testing.NSubstitute.Extensions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NUnit.Framework;
using System;

namespace LazyCache.Testing.NSubstitute.Tests {
    [TestFixture]
    public class Tests : LazyCache.Testing.Common.Tests.Tests {
        protected override void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult) {
            mockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            mockedCache = MockFactory.CreateCachingServiceMock();
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
    }
}