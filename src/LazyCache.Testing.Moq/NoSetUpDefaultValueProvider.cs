using System;
using System.Linq;
using LazyCache.Testing.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace LazyCache.Testing.Moq {
    /// <summary>
    /// Default value provider for methods that have not been set up on a lazy cache mock.
    /// </summary>
    public class NoSetUpDefaultValueProvider : DefaultValueProvider {
        /// <summary>
        ///     Checks the current method invocation on the mock;
        ///     if GetOrAdd or GetOrAddAsync was invoked the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </summary>
        /// <param name="type">The type of the requested default value.</param>
        /// <param name="mock">The mock on which an unexpected invocation has occurred.</param>
        /// <returns>
        ///     if GetOrAdd or GetOrAddAsync was invoked the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </returns>
        protected override object GetDefaultValue(Type type, Mock mock) {
            var lastInvocation = mock.Invocations.Last();
            if (!lastInvocation.Method.Name.StartsWith("GetOrAdd", StringComparison.CurrentCultureIgnoreCase))
                return type.GetDefaultValue();
            
            var func = (Func<ICacheEntry, object>)lastInvocation.Arguments[1];
            var result = func(new CacheEntryFake());
            return result;
        }
    }
}