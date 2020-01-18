using System;
using LazyCache.Testing.Common.Helpers;
using LazyCache.Testing.NSubstitute.Extensions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace LazyCache.Testing.NSubstitute.PackageVerification.Tests
{
    [TestFixture]
    public class ReadmeTests
    {
        [SetUp]
        public virtual void SetUp()
        {
            //LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public virtual void GetOrAddWithSetUp_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = Create.MockedCachingService();
            mockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void MinimumViableInterface_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = Create.MockedCachingService();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}