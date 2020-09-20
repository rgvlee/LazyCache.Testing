using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using rgvlee.Core.Common.Helpers;

namespace LazyCache.Testing.NSubstitute.PackageVerification.Tests
{
    [TestFixture]
    public class ReadmeTests
    {
        [SetUp]
        public void SetUp()
        {
            LoggingHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        }

        [Test]
        public void Example1()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = Create.MockedCachingService();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void Example2()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = Create.MockedCachingService();
            mockedCache.Add(cacheEntryKey, expectedResult);

            var actualResult = mockedCache.Get<string>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void Example3()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = Create.MockedCachingService();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            mockedCache.Received(1).GetOrAdd(cacheEntryKey, Arg.Any<Func<ICacheEntry, string>>(), Arg.Any<MemoryCacheEntryOptions>());
        }
    }
}