﻿using System;
using System.Linq;
using LazyCache.Testing.Extensions;
using LazyCache.Testing.NSubstitute.Extensions;
using NSubstitute.Core;

namespace LazyCache.Testing.NSubstitute {
    /// <summary>
    /// Handler for methods that have not been set up on a lazy cache mock.
    /// </summary>
    public class NoSetUpHandler : ICallHandler {
        private readonly IAppCache _cachingServiceMock;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cachingServiceMock">An instance of the caching service mock.</param>
        public NoSetUpHandler(IAppCache cachingServiceMock) {
            _cachingServiceMock = cachingServiceMock;
        }

        /// <summary>
        ///     Checks the current method invocation on the mock;
        ///     if Add was invoked the unexpected match is set up;
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </summary>
        /// <param name="call"></param>
        /// <returns>
        ///     if Add was invoked the unexpected match is set up;
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </returns>
        public RouteAction Handle(ICall call) {
            Console.WriteLine("NoSetUpHandler invoked");

            var methodInfo = call.GetMethodInfo();
            var args = call.GetArguments();
            
            if (methodInfo.Name.StartsWith("Add", StringComparison.CurrentCultureIgnoreCase)){
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();
                var item = args[1];
                var itemType = methodInfo.GetParameters()[1].ParameterType;

                var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntry"));
                method.MakeGenericMethod(itemType).Invoke(null, new object[] { _cachingServiceMock, key, item });

                return RouteAction.Return(null);
            }

            if(methodInfo.Name.StartsWith("GetOrAdd", StringComparison.CurrentCultureIgnoreCase))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();

                var funcType = methodInfo.GetParameters()[1].ParameterType;
                Console.WriteLine($"{funcType} methods: ");
                foreach (var mi in funcType.GetMethods()) {
                    Console.WriteLine(mi);
                }

                var item = funcType.GetMethod("Invoke").Invoke(args[1], new []{ new CacheEntryFake(key) });

                var itemType = item.GetType();

                var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntry"));
                method.MakeGenericMethod(itemType).Invoke(null, new object[] { _cachingServiceMock, key, item });
                
                return RouteAction.Return(item);
            }

            if (methodInfo.ReturnType == typeof(void)) {
                return RouteAction.Return(null);
            }

            return RouteAction.Return(methodInfo.ReturnType.GetDefaultValue());
        }
    }
}
