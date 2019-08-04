using System;
using System.Linq;
using LazyCache.Testing.Extensions;
using Moq;

namespace LazyCache.Testing.Moq {
    /// <summary>
    /// Default value provider for methods that have not been set up on a lazy cache mock.
    /// </summary>
    public class NoSetUpDefaultValueProvider : DefaultValueProvider {
        private readonly Mock<IAppCache> _cachingServiceMock;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cachingServiceMock">An instance of the caching service mock.</param>
        public NoSetUpDefaultValueProvider(Mock<IAppCache> cachingServiceMock) {
            _cachingServiceMock = cachingServiceMock;
        }

        /// <summary>
        ///     Checks the current method invocation on the mock;
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </summary>
        /// <param name="type">The type of the requested default value.</param>
        /// <param name="mock">The mock on which an unexpected invocation has occurred.</param>
        /// <returns>
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </returns>
        protected override object GetDefaultValue(Type type, Mock mock) {
            Console.WriteLine("NoSetUpDefaultValueProvider invoked");

            var lastInvocation = mock.Invocations.Last();
            var methodInfo = lastInvocation.Method;

            var args = lastInvocation.Arguments;
            
            if (methodInfo.Name.StartsWith("GetOrAdd", StringComparison.CurrentCultureIgnoreCase)) {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();

                var funcType = methodInfo.GetParameters()[1].ParameterType;
                Console.WriteLine($"{funcType} methods: ");
                foreach (var mi in funcType.GetMethods()) {
                    Console.WriteLine(mi);
                }

                var item = funcType.GetMethod("Invoke").Invoke(args[1], new object[] { new CacheEntryFake(key) });

                var itemType = item.GetType();

                var method = typeof(LazyCache.Testing.Moq.Extensions.MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntry"));
                method.MakeGenericMethod(itemType).Invoke(null, new object[] { _cachingServiceMock, key, item });

                return item;
            }
            
            if (methodInfo.ReturnType == typeof(void)) {
                return null;
            }

            return methodInfo.ReturnType.GetDefaultValue();
        }
    }
}