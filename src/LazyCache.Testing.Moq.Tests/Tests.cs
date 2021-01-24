using AutoFixture;
using LazyCache.Testing.Common.Tests;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace LazyCache.Testing.Moq.Tests
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

            Mock.Get(CachingService).Verify(m => m.Add(cacheEntryKey, expectedResult, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
            Mock.Get(CachingService).Verify(m => m.Get<TestObject>(cacheEntryKey), Times.Once);
        }
    }
}