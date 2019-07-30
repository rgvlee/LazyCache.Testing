using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace LazyCache.Testing.Moq.Extensions {
    /// <summary>
    /// Extensions for mocks.
    /// </summary>
    public static class MockExtensions {
        /// <summary>
        /// Sets up a cache entry.
        /// </summary>
        /// <typeparam name="T">The cache entry value type.</typeparam>
        /// <param name="cachingServiceMock">The caching service mock instance.</param>
        /// <param name="key">The cache entry key.</param>
        /// <param name="value">The cache entry value.</param>
        /// <returns>The cache entry value.</returns>
        public static Mock<IAppCache> SetUpCacheEntry<T>(this Mock<IAppCache> cachingServiceMock, string key, T value) {
            Console.WriteLine($"Setting up cache item '{key}': '{value.ToString()}'");

            cachingServiceMock.Setup(m => m.Add<T>(
                    It.Is<string>(s => s.Equals(key)),
                    It.IsAny<T>(),
                    It.IsAny<MemoryCacheEntryOptions>()))
                .Callback(() => Console.WriteLine("Cache Add invoked"));

            cachingServiceMock.Setup(m => m.Get<T>(
                    It.Is<string>(s => s.Equals(key))))
                .Callback(() => Console.WriteLine("Cache Get invoked"))
                .Returns(value);

            cachingServiceMock.Setup(m => m.GetOrAdd<T>(
                    It.IsAny<string>(),
                    It.IsAny<Func<ICacheEntry, T>>()))
                .Callback(() => Console.WriteLine("Cache GetOrAdd invoked"))
                .Returns(value);

            cachingServiceMock.Setup(m => m.GetAsync<T>(
                    It.Is<string>(s => s.Equals(key))))
                .Callback(() => Console.WriteLine("Cache GetAsync invoked"))
                .Returns(Task.FromResult(value));

            cachingServiceMock.Setup(m => m.GetOrAddAsync<T>(
                    It.IsAny<string>(),
                    It.IsAny<Func<ICacheEntry, Task<T>>>()))
                .Callback(() => Console.WriteLine("Cache GetOrAddAsync invoked"))
                .Returns(Task.FromResult(value));

            cachingServiceMock.Setup(m => m.Remove(
                    It.Is<string>(s => s.Equals(key))))
                .Callback(() => Console.WriteLine("Cache Remove invoked"));

            return cachingServiceMock;
        }
    }
}
