using System;
using System.Linq;
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

        private static readonly MethodInfo _itIsMethodWithExpressionOfFuncOfTValueAndBoolArgument = typeof(It).GetMethods()
            .Single(x => x.ToString().Equals("TValue Is[TValue](System.Linq.Expressions.Expression`1[System.Func`2[TValue,System.Boolean]])"));

        private static readonly MethodInfo _itIsAnyMethod = typeof(It).GetMethods().Single(x => x.Name.Equals(nameof(It.IsAny)));

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

        internal static MethodInfo ItIsMethodWithExpressionOfFuncOfTValueAndBoolArgument(Type type)
        {
            return _itIsMethodWithExpressionOfFuncOfTValueAndBoolArgument.MakeGenericMethod(type);
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