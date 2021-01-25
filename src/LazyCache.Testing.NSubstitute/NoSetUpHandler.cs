using System.Linq;
using System.Threading.Tasks;
using LazyCache.Testing.Common;
using Microsoft.Extensions.Logging;
using NSubstitute.Core;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;
using ProjectReflectionShortcuts = LazyCache.Testing.NSubstitute.Helpers.ReflectionShortcuts;
using CoreReflectionShortcuts = rgvlee.Core.Common.Helpers.ReflectionShortcuts;

namespace LazyCache.Testing.NSubstitute
{
    /// <summary>
    ///     Handler for methods that have not been set up on a lazy cache mock.
    /// </summary>
    internal class NoSetUpHandler : ICallHandler
    {
        private static readonly ILogger<NoSetUpHandler> Logger = LoggingHelper.CreateLogger<NoSetUpHandler>();

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

            if (methodInfo.Name.Equals("Add"))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();
                var value = args[1];

                ProjectReflectionShortcuts.SetUpCacheEntryMethod(value.GetType()).Invoke(null, new[] { _mockedCachingService, key, value });

                return RouteAction.Return(null);
            }

            if (methodInfo.Name.Equals("GetOrAdd"))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();
                var value = args[1].GetType().GetMethod("Invoke").Invoke(args[1], new object[] { new CacheEntryFake(key) });

                ProjectReflectionShortcuts.SetUpCacheEntryMethod(value.GetType()).Invoke(null, new[] { _mockedCachingService, key, value });

                return RouteAction.Return(value);
            }

            if (methodInfo.Name.Equals("GetOrAddAsync"))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();
                var task = args[1].GetType().GetMethod("Invoke").Invoke(args[1], new object[] { new CacheEntryFake(key) });
                var taskResult = task.GetType().GetProperty("Result").GetValue(task);

                ProjectReflectionShortcuts.SetUpCacheEntryMethod(taskResult.GetType()).Invoke(null, new[] { _mockedCachingService, key, taskResult });

                return RouteAction.Return(task);
            }

            //void method
            if (methodInfo.ReturnType == typeof(void))
            {
                return RouteAction.Return(null);
            }

            //Return default values
            if (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var genericArgument = methodInfo.ReturnType.GetGenericArguments().Single();
                var defaultValue = genericArgument.GetDefaultValue();

                return RouteAction.Return(CoreReflectionShortcuts.TaskFromResultMethod(genericArgument).Invoke(null, new[] { defaultValue }));
            }

            return RouteAction.Return(methodInfo.ReturnType.GetDefaultValue());
        }
    }
}