using System;
using System.Linq;
using System.Reflection;
using LazyCache.Testing.NSubstitute.Extensions;

namespace LazyCache.Testing.NSubstitute.Helpers
{
    internal static class ReflectionShortcuts
    {
        private static readonly MethodInfo _setUpCacheEntryMethod =
            typeof(MockExtensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(mi => mi.Name.Equals("SetUpCacheEntry"));

        private static readonly MethodInfo _setUpCacheEntryGetMethod =
            typeof(MockExtensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(mi => mi.Name.Equals("SetUpCacheEntryGet"));

        internal static readonly MethodInfo GetOrAddWithMemoryCacheEntryOptionsMethod = typeof(IAppCache).GetMethods()
            .SingleOrDefault(x =>
                x.ToString()
                    .Equals(
                        "T GetOrAdd[T](System.String, System.Func`2[Microsoft.Extensions.Caching.Memory.ICacheEntry,T], Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions)"));

        internal static readonly MethodInfo GetOrAddAsyncWithMemoryCacheEntryOptionsMethod = typeof(IAppCache).GetMethods()
            .SingleOrDefault(x =>
                x.ToString()
                    .Equals(
                        "System.Threading.Tasks.Task`1[T] GetOrAddAsync[T](System.String, System.Func`2[Microsoft.Extensions.Caching.Memory.ICacheEntry,System.Threading.Tasks.Task`1[T]], Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions)"));

        internal static MethodInfo SetUpCacheEntryMethod(Type type)
        {
            return _setUpCacheEntryMethod.MakeGenericMethod(type);
        }

        internal static MethodInfo SetUpCacheEntryGetMethod(Type type)
        {
            return _setUpCacheEntryGetMethod.MakeGenericMethod(type);
        }
    }
}