using LazyCache.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace LazyCache.Testing.Common.Tests {
    [TestFixture]
    public abstract class TestBase {
        protected static readonly ILogger<TestBase> Logger = LoggerHelper.CreateLogger<TestBase>();

        protected IAppCache MockedCache;

        protected abstract void SetUpCacheEntry<T>(string cacheEntryKey, T expectedResult);
            
        [SetUp]
        public virtual void SetUp() {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Debug);
            LoggerHelper.LoggerFactory = loggerFactory;
        }

        [Test]
        public virtual void GetOrAddWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            SetUpCacheEntry(cacheEntryKey, expectedResult);
            
            var actualResult = MockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));
            
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            SetUpCacheEntry(cacheEntryKey, expectedResult);
            
            var actualResult = await MockedCache.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void GetWithNoSetUp_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            
            var actualResult = MockedCache.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(default(Guid)));
        }

        [Test]
        public void GetOrAddWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();

            var actualResult = MockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult);
            });
        }

        [Test]
        public void GetOrAddWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            var actualResult = MockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult);
            });
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            
            var actualResult = await MockedCache.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task GetOrAddAsyncWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();
            
            var actualResult = await MockedCache.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetThenGetOrAddThenGetWithNoSetUp_TestObject_ReturnsExpectedResults() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = default(TestObject);
            var expectedResult2 = new TestObject();
            var expectedResult3 = expectedResult2;

            var actualResult1 = MockedCache.Get<TestObject>(cacheEntryKey);
            var actualResult2 = MockedCache.GetOrAdd(cacheEntryKey, () => expectedResult2, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = MockedCache.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.IsNull(actualResult1);
                Assert.AreEqual(expectedResult1, actualResult1);

                Assert.AreEqual(expectedResult2, actualResult2);

                Assert.IsNotNull(actualResult3);
                Assert.AreEqual(expectedResult3, actualResult3);
            });
        }

        [Test]
        public void GetThenGetOrAddThenGetWithSetUp_TestObject_ReturnsExpectedResults() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();

            SetUpCacheEntry(cacheEntryKey, expectedResult);

            var actualResult1 = MockedCache.Get<TestObject>(cacheEntryKey);
            var actualResult2 = MockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = MockedCache.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult, actualResult1);
                Assert.AreEqual(expectedResult, actualResult2);
                Assert.AreEqual(expectedResult, actualResult3);
            });
        }

        [Test]
        public virtual void MinimumViableInterface_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            
            var actualResult = MockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void AddThenGetWithNoSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = Guid.NewGuid();
            
            Logger.LogDebug("Add invocation started");
            MockedCache.Add(cacheEntryKey, expectedResult);
            Logger.LogDebug("Add invocation finished");
            var actualResult = MockedCache.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void AddThenGetWithNoSetUp_TestObject_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult = new TestObject();
            
            MockedCache.Add(cacheEntryKey, expectedResult);

            var actualResult = MockedCache.Get<TestObject>(cacheEntryKey);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public virtual void AddThenGetWithSetUp_Guid_ReturnsExpectedResult() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();
            var expectedResult2 = Guid.NewGuid();

            SetUpCacheEntry(cacheEntryKey, expectedResult1);
            
            var actualResult1 = MockedCache.Get<Guid>(cacheEntryKey);
            Assert.AreEqual(expectedResult1, actualResult1);

            MockedCache.Add(cacheEntryKey, expectedResult2);
            var actualResult2 = MockedCache.Get<Guid>(cacheEntryKey);

            Assert.AreEqual(expectedResult2, actualResult2);
        }

        [Test]
        public virtual void RemoveWithSetUp_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            var expectedResult1 = Guid.NewGuid();

            SetUpCacheEntry(cacheEntryKey, expectedResult1);
            
            var actualResult1 = MockedCache.Get<Guid>(cacheEntryKey);
            MockedCache.Remove(cacheEntryKey);
            var actualResult2 = MockedCache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.AreEqual(expectedResult1, actualResult1);
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }

        [Test]
        public virtual void RemoveWithNoSetUp_DoesNothing() {
            var cacheEntryKey = "SomethingInTheCache";

            Assert.DoesNotThrow(() => {
                MockedCache.Remove(cacheEntryKey);
            });
        }

        [Test]
        public virtual void RemoveWithNoSetUp_Guid_ReturnsDefaultValue() {
            var cacheEntryKey = "SomethingInTheCache";
            
            var actualResult1 = MockedCache.Get<Guid>(cacheEntryKey);
            MockedCache.Remove(cacheEntryKey);
            var actualResult2 = MockedCache.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() => {
                Assert.That(actualResult1, Is.EqualTo(default(Guid)));
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }
    }
}