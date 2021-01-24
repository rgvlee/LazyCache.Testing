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

        internal static readonly MethodInfo GetOrAddWithMemoryCacheEntryOptionsMethod =
            typeof(IAppCache).GetMethods().SingleOrDefault(x => x.Name.Equals("GetOrAdd") && x.GetParameters().Length == 3);

        internal static readonly MethodInfo GetOrAddAsyncWithMemoryCacheEntryOptionsMethod =
            typeof(IAppCache).GetMethods().SingleOrDefault(x => x.Name.Equals("GetOrAddAsync") && x.GetParameters().Length == 3);

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