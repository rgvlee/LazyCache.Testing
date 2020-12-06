using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using rgvlee.Core.Common.Helpers;

namespace LazyCache.Testing.Common.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        [SetUp]
        public virtual void SetUp()
        {
            LoggingHelper.LoggerFactory.AddConsole(LogLevel.Trace);
            //LoggingHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
        }

        protected static readonly ILogger<TestBase> Logger = LoggingHelper.CreateLogger<TestBase>();

        protected IAppCache CachingService;

        protected abstract void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult);

        [Test]
        public virtual void Add_Null_ThrowsException()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = default(TestObject);

            Assert.Multiple(() =>
            {
                var ex = Assert.Throws<ArgumentNullException>(() =>
                {
                    CachingService.Add(cacheEntryKey, expectedResult);
                });
                Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
            });
        }

        [Test]
        public virtual void AddThenGetWithNoSetUp_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            Logger.LogDebug("Add invocation started");
            CachingService.Add(cacheEntryKey, expectedResult);
            Logger.LogDebug("Add invocation finished");
            var actualResult = CachingService.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void AddThenGetWithNoSetUp_TestObject_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            CachingService.Add(cacheEntryKey, expectedResult);

            var actualResult = CachingService.Get<TestObject>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void AddThenGetWithSetUp_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();
            var expectedResult2 = Guid.NewGuid();

            SetUpCacheEntry(cacheEntryKey, expectedResult1);

            var actualResult1 = CachingService.Get<Guid>(cacheEntryKey);
            Assert.AreEqual(expectedResult1, actualResult1);

            CachingService.Add(cacheEntryKey, expectedResult2);
            var actualResult2 = CachingService.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult2, actualResult2);
        }

        [Test]
        public virtual void AddWithExpirationTokenThenGet_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var cacheEntryOptions = new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(cts.Token));

            Logger.LogDebug("Add invocation started");
            CachingService.Add(cacheEntryKey, expectedResult, cacheEntryOptions);
            Logger.LogDebug("Add invocation finished");
            var actualResult = CachingService.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void AddWithPostEvictionCallbackThenGet_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var cacheEntryOptions = new MemoryCacheEntryOptions().RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                Logger.LogDebug("PostEvictionCallback invoked");
            });

            Logger.LogDebug("Add invocation started");
            CachingService.Add(cacheEntryKey, expectedResult, cacheEntryOptions);
            Logger.LogDebug("Add invocation finished");
            var actualResult = CachingService.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual async Task GetOrAddAsync_GuidWithNoSetUp_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            var cacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) };

            var actualResult = await (withCacheEntryOptions
                ? CachingService.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult), cacheEntryOptions)
                : CachingService.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult)));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual async Task GetOrAddAsync_TestObjectWithNoSetUp_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();
            var cacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) };

            var actualResult = await (withCacheEntryOptions
                ? CachingService.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult), cacheEntryOptions)
                : CachingService.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult)));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual async Task GetOrAddAsync_GuidWithSetUp_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            SetUpCacheEntry(cacheEntryKey, expectedResult);
            var cacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) };

            var actualResult = await (withCacheEntryOptions
                ? CachingService.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult), cacheEntryOptions)
                : CachingService.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult)));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual void GetOrAdd_GuidWithNoSetUp_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            var cacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) };

            var actualResult = withCacheEntryOptions
                ? CachingService.GetOrAdd(cacheEntryKey, () => expectedResult, cacheEntryOptions)
                : CachingService.GetOrAdd(cacheEntryKey, () => expectedResult);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual void GetOrAdd_TestObjectWithNoSetUp_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();
            var cacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) };

            var actualResult = withCacheEntryOptions
                ? CachingService.GetOrAdd(cacheEntryKey, () => expectedResult, cacheEntryOptions)
                : CachingService.GetOrAdd(cacheEntryKey, () => expectedResult);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual void GetOrAdd_GuidWithSetUp_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            SetUpCacheEntry(cacheEntryKey, expectedResult);
            var cacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) };

            var actualResult = withCacheEntryOptions
                ? CachingService.GetOrAdd(cacheEntryKey, () => expectedResult, cacheEntryOptions)
                : CachingService.GetOrAdd(cacheEntryKey, () => expectedResult);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void GetOrCreateThenRemoveWithNoSetUp_Guid_ReturnsDefaultValue()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();

            var actualResult1 = CachingService.GetOrAdd(cacheEntryKey, () => expectedResult1);
            CachingService.Remove(cacheEntryKey);
            var actualResult2 = CachingService.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult1));
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }

        [Test]
        public virtual void GetThenGetOrAddThenGetWithNoSetUp_TestObject_ReturnsExpectedResults()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = default(TestObject);
            var expectedResult2 = new TestObject();
            var expectedResult3 = expectedResult2;

            var actualResult1 = CachingService.Get<TestObject>(cacheEntryKey);
            var actualResult2 = CachingService.GetOrAdd(cacheEntryKey, () => expectedResult2, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = CachingService.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.IsNull(actualResult1);
                Assert.AreEqual(expectedResult1, actualResult1);

                Assert.AreEqual(expectedResult2, actualResult2);

                Assert.IsNotNull(actualResult3);
                Assert.AreEqual(expectedResult3, actualResult3);
            });
        }

        [Test]
        public virtual void GetThenGetOrAddThenGetWithSetUp_TestObject_ReturnsExpectedResults()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            SetUpCacheEntry(cacheEntryKey, expectedResult);

            var actualResult1 = CachingService.Get<TestObject>(cacheEntryKey);
            var actualResult2 = CachingService.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = CachingService.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedResult, actualResult1);
                Assert.AreEqual(expectedResult, actualResult2);
                Assert.AreEqual(expectedResult, actualResult3);
            });
        }

        [Test]
        public virtual void Get_SpecifiedKeyWithNoSetUp_ReturnsDefaultValue()
        {
            var cacheEntryKey = "SomethingInTheCache";

            var actualResult = CachingService.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(default(Guid)));
        }

        [Test]
        public virtual async Task GetAsync_SpecifiedKeyWithNoSetUp_ReturnsDefaultValue()
        {
            var cacheEntryKey = "SomethingInTheCache";

            var actualResult = await CachingService.GetAsync<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(default(Guid)));
        }

        [Test]
        public virtual void MinimumViableInterface_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var actualResult = CachingService.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void RemoveWithNoSetUp_DoesNothing()
        {
            var cacheEntryKey = "SomethingInTheCache";

            Assert.DoesNotThrow(() =>
            {
                CachingService.Remove(cacheEntryKey);
            });
        }

        [Test]
        public virtual void RemoveWithNoSetUp_Guid_ReturnsDefaultValue()
        {
            var cacheEntryKey = "SomethingInTheCache";

            var actualResult1 = CachingService.Get<Guid>(cacheEntryKey);
            CachingService.Remove(cacheEntryKey);
            var actualResult2 = CachingService.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(default(Guid)));
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }

        [Test]
        public virtual void RemoveWithSetUp_Guid_ReturnsDefaultValue()
        {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();

            SetUpCacheEntry(cacheEntryKey, expectedResult1);

            var actualResult1 = CachingService.Get<Guid>(cacheEntryKey);
            CachingService.Remove(cacheEntryKey);
            var actualResult2 = CachingService.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedResult1, actualResult1);
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }
    }
}