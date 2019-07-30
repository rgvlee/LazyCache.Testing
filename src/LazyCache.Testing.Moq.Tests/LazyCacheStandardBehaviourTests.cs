using LazyCache.Mocks;
using NUnit.Framework;
using System;

namespace LazyCache.Testing.Moq.Tests {
    [TestFixture]
    class LazyCacheStandardBehaviourTests {
        [Test]
        public void GetOrAddWithReturnsDefaultSetUp_Guid_ReturnsExpectedResult() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var cacheMock = MockFactory.CreateCachingServiceMock();
            cacheMock.SetReturnsDefault(expectedResult);
            var mockedCache = cacheMock.Object;

            var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetWithBuiltInFake_Guid_ReturnsNull() {
            var cacheKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid().ToString();

            var mockedCache = new MockCachingService();

            var actualResult = mockedCache.Get<string>(cacheKey);

            Assert.Multiple(() => {
                Assert.IsNotNull(expectedResult);
                Assert.IsNull(actualResult);
            });
        }

        //This test does not work with LazyCache v2.0.0
        //[Test]
        //public void AddAndGetWithBuiltInFake_Guid_ReturnsNull() {
        //    var cacheKey = "SomethingInTheCache";
        //    var expectedResult = Guid.NewGuid().ToString();

        //    var mockedCache = new MockCachingService();

        //    mockedCache.Add<string>(cacheKey, expectedResult);
        //    var actualResult = mockedCache.Get<string>(cacheKey);

        //    Assert.Multiple(() => {
        //        Assert.IsNotNull(expectedResult);
        //        Assert.IsNull(actualResult);
        //    });
        //}
    }
}
