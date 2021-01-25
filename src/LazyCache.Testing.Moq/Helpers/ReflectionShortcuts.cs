using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using MockExtensions = LazyCache.Testing.Moq.Extensions.MockExtensions;

namespace LazyCache.Testing.Moq.Helpers
{
    internal static class ReflectionShortcuts
    {
        private static readonly MethodInfo _setUpCacheEntryMethod =
            typeof(MockExtensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(mi => mi.Name.Equals("SetUpCacheEntry"));

        private static readonly MethodInfo _setUpCacheEntryGetMethod =
            typeof(MockExtensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(mi => mi.Name.Equals("SetUpCacheEntryGet"));

        //private static readonly MethodInfo _itIsMethod = typeof(It).GetMethods().Single(x => x.Name.Equals(nameof(It.Is)) && x.IsGenericMethod && x.GetParameters().Length == 1);
        private static readonly MethodInfo _itIsMethod = typeof(It).GetMethods()
            .Single(x => x.Name.Equals(nameof(It.Is)) &&
                         x.IsGenericMethod &&
                         x.GetParameters().Length == 1 &&
                         x.GetParameters()[0].ParameterType != typeof(Expression<Func<object, Type, bool>>));

        private static readonly MethodInfo _itIsAnyMethod =
            typeof(It).GetMethods().Single(x => x.Name.Equals(nameof(It.IsAny)) && x.IsGenericMethod && x.GetParameters().Length == 0);

        internal static readonly MethodInfo GetOrAddWithMemoryCacheEntryOptionsMethod =
            typeof(IAppCache).GetMethods().SingleOrDefault(x => x.Name.Equals("GetOrAdd") && x.GetParameters().Length == 3);

        internal static readonly MethodInfo GetOrAddAsyncWithMemoryCacheEntryOptionsMethod =
            typeof(IAppCache).GetMethods().SingleOrDefault(x => x.Name.Equals("GetOrAddAsync") && x.GetParameters().Length == 3);

        internal static MethodInfo ItIsMethod(Type type)
        {
            return _itIsMethod.MakeGenericMethod(type);
        }

        internal static MethodInfo ItIsAnyMethod(Type type)
        {
            return _itIsAnyMethod.MakeGenericMethod(type);
        }

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