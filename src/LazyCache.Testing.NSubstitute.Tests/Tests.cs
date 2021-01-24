using AutoFixture;
using LazyCache.Testing.Common.Tests;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NUnit.Framework;

namespace LazyCache.Testing.NSubstitute.Tests
{
    public class Tests : BaseForTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CachingService = Create.MockedCachingService();
        }

        [Test]
        public virtual void AddThenGet_GuidKeyAndTestObjectValue_AddAndGetEachInvokedOnce()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<TestObject>();

            //Act
            CachingService.Add(cacheEntryKey, expectedResult);
            var actualResult = CachingService.Get<TestObject>(cacheEntryKey);

            CachingService.Received(1).Add(cacheEntryKey, expectedResult, Arg.Any<MemoryCacheEntryOptions>());
            CachingService.Received(1).Get<TestObject>(cacheEntryKey);
        }
    }
}