using System;
using NUnit.Framework;

namespace LazyCache.DefaultBehaviour.Tests
{
    [TestFixture]
    public class CachingServiceTests
    {
        [Test]
        public void AddThenAddThenGet_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();
            var expectedResult2 = Guid.NewGuid();

            var cache = new CachingService();
            cache.Remove(cacheEntryKey);

            cache.Add(cacheEntryKey, expectedResult1, null);
            cache.Add(cacheEntryKey, expectedResult2, null);
            var actualResult = cache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedResult2, actualResult);
            });
        }

        [Test]
        public void AddThenGet_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cache = new CachingService();
            cache.Remove(cacheEntryKey);

            cache.Add(cacheEntryKey, expectedResult, null);
            var actualResult = cache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedResult, actualResult);
            });
        }

        [Test]
        public void Get_Guid_ReturnsDefaultValue()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cache = new CachingService();
            cache.Remove(cacheEntryKey);

            var actualResult = cache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(expectedResult);
                Assert.That(actualResult, Is.EqualTo(default(Guid)));
            });
        }
    }
}