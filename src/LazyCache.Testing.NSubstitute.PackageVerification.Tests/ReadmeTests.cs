using System;
using AutoFixture;
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
            LoggingHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            Fixture = new Fixture();
        }

        protected Fixture Fixture;

        [Test]
        public void Example1()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();

            var mockedCache = Create.MockedCachingService();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Example2()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();

            var mockedCache = Create.MockedCachingService();
            mockedCache.Add(cacheEntryKey, expectedResult);

            var actualResult = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Example3()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();

            var mockedCache = Create.MockedCachingService();

            var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            mockedCache.Received(1).GetOrAdd(cacheEntryKey, Arg.Any<Func<ICacheEntry, Guid>>(), Arg.Any<MemoryCacheEntryOptions>());
        }
    }
}