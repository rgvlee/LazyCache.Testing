using LazyCache.Testing.Common.Helpers;
using LazyCache.Testing.NSubstitute.Extensions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;

namespace LazyCache.Testing.NSubstitute.PackageVerification.Tests {
    public class ReadmeTests {
        [SetUp]
        public virtual void SetUp() {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = MockFactory.CreateCachingServiceMock();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetOrAddWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetUpCacheEntry(cacheEntryKey, expectedResult);

            var actualResult = cacheMock.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}