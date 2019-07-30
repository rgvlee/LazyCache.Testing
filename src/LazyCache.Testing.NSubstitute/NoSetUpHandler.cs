using System;
using System.Linq;
using LazyCache.Testing.Extensions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute.Core;

namespace LazyCache.Testing.NSubstitute {
    /// <summary>
    /// Handler for methods that have not been set up on a lazy cache mock.
    /// </summary>
    public class NoSetUpHandler : ICallHandler{
        /// <summary>
        ///     Checks the current method invocation on the mock;
        ///     if GetOrAdd or GetOrAddAsync was invoked the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </summary>
        /// <param name="call"></param>
        /// <returns>
        ///     if GetOrAdd or GetOrAddAsync was invoked the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </returns>
        public RouteAction Handle(ICall call) {
            var methodInfo = call.GetMethodInfo();
            
            if (!methodInfo.Name.StartsWith("GetOrAdd", StringComparison.CurrentCultureIgnoreCase))
                return RouteAction.Return(methodInfo.ReturnType.GetDefaultValue());

            var func = (Func<ICacheEntry, object>)call.GetArguments().ElementAt(1);
            var result = func(new CacheEntryFake());
            return RouteAction.Return(result);
        }
    }
}
