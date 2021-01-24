﻿using System;
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

        private static readonly MethodInfo _itIsMethod = typeof(It).GetMethod(nameof(It.Is));

        private static readonly MethodInfo _itIsAnyMethod = typeof(It).GetMethod(nameof(It.IsAny));

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