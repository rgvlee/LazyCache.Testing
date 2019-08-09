using LazyCache.Mocks;
using NUnit.Framework;
using System;

namespace LazyCache.DefaultBehaviour.Tests {
    [TestFixture]
    public class MockCachingServiceTests {
        [Test]
        public void GetWithBuiltInFake_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            
            var mockedCache = new MockCachingService();

            var actualResult = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.That(actualResult, Is.EqualTo(default(Guid)));
            });
        }
        
        [Test]
        public void AddThenGetWithBuiltInFake_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            var cacheValue = Guid.NewGuid();

            var mockedCache = new MockCachingService();

            mockedCache.Add(cacheEntryKey, cacheValue, null);
            var actualResult = mockedCache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.That(actualResult, Is.EqualTo(default(Guid)));
            });
        }
    }
}
