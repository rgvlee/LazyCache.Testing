using LazyCache.Testing.Common.Helpers;
using LazyCache.Testing.Moq.Extensions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;

namespace LazyCache.Testing.Moq.PackageVerification.Tests {
    public class ReadmeTests {
        [SetUp]
        public virtual void SetUp() {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = MockFactory.CreateMockedCachingService();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetOrAddWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetOrAddWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }
    }
}