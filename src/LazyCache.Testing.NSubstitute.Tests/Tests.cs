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

            SUT = Create.MockedCachingService();
        }

        [Test]
        public virtual void AddThenGet_TestObject_AddAndGetEachInvokedOnce()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<TestObject>();

            SUT.Add(cacheEntryKey, expectedResult);
            var actualResult = SUT.Get<TestObject>(cacheEntryKey);

            SUT.Received(1).Add(cacheEntryKey, expectedResult, Arg.Any<MemoryCacheEntryOptions>());
            SUT.Received(1).Get<TestObject>(cacheEntryKey);
        }
    }
}