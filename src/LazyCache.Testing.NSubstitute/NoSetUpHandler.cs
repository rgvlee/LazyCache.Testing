using System.Linq;
using LazyCache.Testing.Common;
using LazyCache.Testing.Common.Extensions;
using LazyCache.Testing.Common.Helpers;
using LazyCache.Testing.NSubstitute.Extensions;
using Microsoft.Extensions.Logging;
using NSubstitute.Core;

namespace LazyCache.Testing.NSubstitute
{
    /// <summary>
    ///     Handler for methods that have not been set up on a lazy cache mock.
    /// </summary>
    internal class NoSetUpHandler : ICallHandler
    {
        private static readonly ILogger<NoSetUpHandler> Logger = LoggerHelper.CreateLogger<NoSetUpHandler>();

        private readonly IAppCache _mockedCachingService;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="mockedCachingService">The mocked caching service.</param>
        public NoSetUpHandler(IAppCache mockedCachingService)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));

            _mockedCachingService = mockedCachingService;
        }

        /// <summary>
        ///     Checks the last method invocation on the mock;
        ///     if Add was invoked the unexpected match is set up;
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be
        ///     returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </summary>
        /// <param name="call"></param>
        /// <returns>
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be
        ///     returned;
        ///     otherwise the default value for the specified type will be returned if the last method invocation has a return
        ///     type.
        /// </returns>
        public RouteAction Handle(ICall call)
        {
            Logger.LogDebug("NoSetUpHandler invoked");

            var methodInfo = call.GetMethodInfo();
            var args = call.GetArguments();

            if (methodInfo.Name.StartsWith("Add"))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();
                var value = args[1];
                var valueType = methodInfo.GetParameters()[1].ParameterType;

                var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntry"));
                method.MakeGenericMethod(valueType).Invoke(null, new[] { _mockedCachingService, key, value });

                return RouteAction.Return(null);
            }

            if (methodInfo.Name.StartsWith("GetOrAdd"))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();

                var funcType = methodInfo.GetParameters()[1].ParameterType;
                Logger.LogDebug($"{funcType} methods: ");
                foreach (var mi in funcType.GetMethods())
                {
                    Logger.LogDebug(mi.ToString());
                }

                var value = funcType.GetMethod("Invoke").Invoke(args[1], new[] { new CacheEntryFake(key) });

                var valueType = value.GetType();

                var method = typeof(MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntry"));
                method.MakeGenericMethod(valueType).Invoke(null, new[] { _mockedCachingService, key, value });

                return RouteAction.Return(value);
            }

            if (methodInfo.ReturnType == typeof(void))
            {
                return RouteAction.Return(null);
            }

            return RouteAction.Return(methodInfo.ReturnType.GetDefaultValue());
        }
    }
}