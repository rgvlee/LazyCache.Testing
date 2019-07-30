using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace LazyCache.Testing.NSubstitute.Extensions {
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
        public static IAppCache SetUpCacheEntry<T>(this IAppCache cachingServiceMock, string key, T value) {
            Console.WriteLine($"Setting up cache item '{key}': '{value.ToString()}'");
            
            cachingServiceMock
                .When(x => x.Add(Arg.Is<string>(s => s.Equals(key)), Arg.Any<T>(), Arg.Any<MemoryCacheEntryOptions>()))
                .Do(x => Console.WriteLine("Cache Add invoked"));
            
            cachingServiceMock.Get<T>(Arg.Is<string>(s => s.Equals(key)))
                .Returns(value)
                .AndDoes(x => Console.WriteLine("Cache Get invoked"));

            cachingServiceMock.GetOrAdd<T>(
                    Arg.Any<string>(),
                    Arg.Any<Func<ICacheEntry, T>>())
                .Returns(value)
                .AndDoes(x => Console.WriteLine("Cache GetOrAdd invoked"));

            cachingServiceMock.GetAsync<T>(
                    Arg.Is<string>(s => s.Equals(key)))
                .Returns(Task.FromResult(value))
                .AndDoes(x => Console.WriteLine("Cache GetAsync invoked"));

            cachingServiceMock.GetOrAddAsync<T>(
                    Arg.Any<string>(),
                    Arg.Any<Func<ICacheEntry, Task<T>>>())
                .Returns(Task.FromResult(value))
                .AndDoes(x => Console.WriteLine("Cache GetOrAddAsync invoked"));

            cachingServiceMock
                .When(x => x.Remove(Arg.Is<string>(s => s.Equals(key))))
                .Do(x => Console.WriteLine("Cache Remove invoked"));
            
            return cachingServiceMock;
        }
    }
}
