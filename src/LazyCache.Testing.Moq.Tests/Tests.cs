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

            SUT = Create.MockedCachingService();
        }

        [Test]
        public virtual void AddThenGet_TestObject_AddAndGetEachInvokedOnce()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<TestObject>();

            SUT.Add(cacheEntryKey, expectedResult);
            var actualResult = SUT.Get<TestObject>(cacheEntryKey);

            Mock.Get(SUT).Verify(m => m.Add(cacheEntryKey, expectedResult, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
            Mock.Get(SUT).Verify(m => m.Get<TestObject>(cacheEntryKey), Times.Once);
        }
    }
}