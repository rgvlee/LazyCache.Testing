using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using rgvlee.Core.Common.Helpers;

namespace LazyCache.Testing.Common.Tests
{
    [TestFixture]
    public abstract class BaseForTests
    {
        [SetUp]
        public virtual void SetUp()
        {
            //LoggingHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
            LoggingHelper.LoggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);

            Fixture = new Fixture();
            Fixture.Customize<MemoryCacheEntryOptions>(x => x.OmitAutoProperties().With(y => y.AbsoluteExpiration, DateTimeOffset.Now.AddDays(1)));
        }

        [TearDown]
        public virtual void TearDown()
        {
            LoggingHelper.LoggerFactory.Dispose();
        }

        protected Fixture Fixture;

        protected IAppCache SUT;

        [Test]
        public virtual void Add_DefaultTestObject_ThrowsException()
        {
            Assert.Multiple(() =>
            {
                var ex = Assert.Throws<ArgumentNullException>(() =>
                {
                    SUT.Add(Fixture.Create<string>(), default(TestObject));
                });

                Assert.That(ex?.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
            });
        }

        [Test]
        public virtual void AddThenGet_Guid_ReturnsExpectedResult()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();

            SUT.Add(cacheEntryKey, expectedResult);
            var actualResult = SUT.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public virtual void AddThenGet_TestObject_ReturnsExpectedResult()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<TestObject>();

            SUT.Add(cacheEntryKey, expectedResult);
            var actualResult = SUT.Get<TestObject>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public virtual void AddThenGet_GuidWithExpirationToken_ReturnsExpectedResult()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();
            var cts = new CancellationTokenSource(TimeSpan.FromDays(1));
            var cacheEntryOptions = Fixture.Create<MemoryCacheEntryOptions>();
            cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cts.Token));

            SUT.Add(cacheEntryKey, expectedResult, cacheEntryOptions);
            var actualResult = SUT.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public virtual void AddThenGet_GuidWithPostEvictionCallback_ReturnsExpectedResult()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();
            var cacheEntryOptions = Fixture.Create<MemoryCacheEntryOptions>();
            cacheEntryOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                Console.WriteLine("PostEvictionCallback invoked");
            });

            SUT.Add(cacheEntryKey, expectedResult, cacheEntryOptions);
            var actualResult = SUT.Get<Guid>(cacheEntryKey);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual async Task GetOrAddAsync_Guid_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();
            var cacheEntryOptions = Fixture.Create<MemoryCacheEntryOptions>();

            var actualResult = await (withCacheEntryOptions
                ? SUT.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult), cacheEntryOptions)
                : SUT.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult)));

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual async Task GetOrAddAsync_TestObject_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<TestObject>();
            var cacheEntryOptions = Fixture.Create<MemoryCacheEntryOptions>();

            var actualResult = await (withCacheEntryOptions
                ? SUT.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult), cacheEntryOptions)
                : SUT.GetOrAddAsync(cacheEntryKey, () => Task.FromResult(expectedResult)));

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual void GetOrAdd_Guid_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();
            var cacheEntryOptions = Fixture.Create<MemoryCacheEntryOptions>();

            var actualResult = withCacheEntryOptions
                ? SUT.GetOrAdd(cacheEntryKey, () => expectedResult, cacheEntryOptions)
                : SUT.GetOrAdd(cacheEntryKey, () => expectedResult);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCase(true)]
        [TestCase(false)]
        public virtual void GetOrAdd_TestObject_ReturnsExpectedResult(bool withCacheEntryOptions)
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<TestObject>();
            var cacheEntryOptions = Fixture.Create<MemoryCacheEntryOptions>();

            var actualResult = withCacheEntryOptions
                ? SUT.GetOrAdd(cacheEntryKey, () => expectedResult, cacheEntryOptions)
                : SUT.GetOrAdd(cacheEntryKey, () => expectedResult);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public virtual void GetOrAddThenRemoveThenGet_Guid_ReturnsExpectedResults()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult1 = Fixture.Create<Guid>();
            var expectedResult2 = default(Guid);

            var actualResult1 = SUT.GetOrAdd(cacheEntryKey, () => expectedResult1);
            SUT.Remove(cacheEntryKey);
            var actualResult2 = SUT.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult1));
                Assert.That(actualResult2, Is.EqualTo(expectedResult2));
            });
        }

        [Test]
        public virtual void GetThenGetOrAddThenGet_TestObject_ReturnsExpectedResults()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult1 = default(TestObject);
            var expectedResult2 = Fixture.Create<TestObject>();
            var expectedResult3 = expectedResult2;

            var actualResult1 = SUT.Get<TestObject>(cacheEntryKey);
            var actualResult2 = SUT.GetOrAdd(cacheEntryKey, () => expectedResult2, DateTimeOffset.Now.AddMinutes(30));
            var actualResult3 = SUT.Get<TestObject>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult1));
                Assert.That(actualResult2, Is.EqualTo(expectedResult2));
                Assert.That(actualResult3, Is.EqualTo(expectedResult3));
            });
        }

        [Test]
        public virtual void Get_ReturnsDefaultValue()
        {
            var actualResult = SUT.Get<Guid>(Fixture.Create<string>());

            Assert.That(actualResult, Is.EqualTo(default(Guid)));
        }

        [Test]
        public virtual async Task GetAsync_ReturnsDefaultValue()
        {
            var actualResult = await SUT.GetAsync<Guid>(Fixture.Create<string>());

            Assert.That(actualResult, Is.EqualTo(default(Guid)));
        }

        [Test]
        public virtual void Remove_DoesNothing()
        {
            Assert.DoesNotThrow(() =>
            {
                SUT.Remove(Fixture.Create<string>());
            });
        }

        [Test]
        public virtual void GetThenRemoveThenGet_ReturnsDefaultValue()
        {
            var cacheEntryKey = Fixture.Create<string>();

            var actualResult1 = SUT.Get<Guid>(cacheEntryKey);
            SUT.Remove(cacheEntryKey);
            var actualResult2 = SUT.Get<Guid>(cacheEntryKey);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(default(Guid)));
                Assert.That(actualResult2, Is.EqualTo(default(Guid)));
            });
        }

        [Test]
        public virtual void GetOrAdd_AddItemFactoryInvokedOnce()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();
            var invocationCounter = 0;
            Func<Guid> func = () =>
            {
                invocationCounter++;
                return expectedResult;
            };
            var options = Fixture.Create<MemoryCacheEntryOptions>();

            var actualResult1 = SUT.GetOrAdd(cacheEntryKey, func, options);
            var actualResult2 = SUT.GetOrAdd(cacheEntryKey, func, options);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(expectedResult));
                Assert.That(invocationCounter, Is.EqualTo(1));
            });
        }

        [Test]
        public virtual async Task GetOrAddAsync_AddItemFactoryInvokedOnce()
        {
            var cacheEntryKey = Fixture.Create<string>();
            var expectedResult = Fixture.Create<Guid>();
            var invocationCounter = 0;
            Func<Task<Guid>> func = () =>
            {
                invocationCounter++;
                return Task.FromResult(expectedResult);
            };
            var options = Fixture.Create<MemoryCacheEntryOptions>();

            var actualResult1 = await SUT.GetOrAddAsync(cacheEntryKey, func, options);
            var actualResult2 = await SUT.GetOrAddAsync(cacheEntryKey, func, options);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(expectedResult));
                Assert.That(invocationCounter, Is.EqualTo(1));
            });
        }
    }
}