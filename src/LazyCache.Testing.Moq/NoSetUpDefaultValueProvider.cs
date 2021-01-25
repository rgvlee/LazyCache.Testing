using System;
using System.Linq;
using System.Threading.Tasks;
using LazyCache.Testing.Common;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;
using ProjectReflectionShortcuts = LazyCache.Testing.Moq.Helpers.ReflectionShortcuts;
using CoreReflectionShortcuts = rgvlee.Core.Common.Helpers.ReflectionShortcuts;

namespace LazyCache.Testing.Moq
{
    /// <summary>
    ///     Default value provider for methods that have not been set up on a lazy cache mock.
    /// </summary>
    internal class NoSetUpDefaultValueProvider : DefaultValueProvider
    {
        private static readonly ILogger<NoSetUpDefaultValueProvider> Logger = LoggingHelper.CreateLogger<NoSetUpDefaultValueProvider>();

        private readonly IAppCache _mockedCachingService;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="mockedCachingService">The mocked caching service.</param>
        public NoSetUpDefaultValueProvider(IAppCache mockedCachingService)
        {
            EnsureArgument.IsNotNull(mockedCachingService, nameof(mockedCachingService));

            _mockedCachingService = mockedCachingService;
        }

        /// <summary>
        ///     Checks the last method invocation on the mock;
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be
        ///     returned;
        ///     otherwise the default value for the specified type will be returned.
        /// </summary>
        /// <param name="type">The type of the requested default value.</param>
        /// <param name="mock">The mock on which an unexpected invocation has occurred.</param>
        /// <returns>
        ///     if GetOrAdd or GetOrAddAsync was invoked the unexpected match is set up and the addItemFactory result will be
        ///     returned;
        ///     otherwise the default value for the specified type will be returned if the last method invocation has a return
        ///     type.
        /// </returns>
        protected override object GetDefaultValue(Type type, Mock mock)
        {
            Logger.LogDebug("NoSetUpDefaultValueProvider invoked");

            var lastInvocation = mock.Invocations.Last();
            var methodInfo = lastInvocation.Method;
            var args = lastInvocation.Arguments;

            if (methodInfo.Name.Equals("GetOrAdd"))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();
                var value = args[1].GetType().GetMethod("Invoke").Invoke(args[1], new object[] { new CacheEntryFake(key) });

                ProjectReflectionShortcuts.SetUpCacheEntryMethod(value.GetType()).Invoke(null, new[] { _mockedCachingService, key, value });

                return value;
            }

            if (methodInfo.Name.Equals("GetOrAddAsync"))
            {
                //We have everything we need to set up a match, so let's do it
                var key = args[0].ToString();
                var task = args[1].GetType().GetMethod("Invoke").Invoke(args[1], new object[] { new CacheEntryFake(key) });
                var taskResult = task.GetType().GetProperty("Result").GetValue(task);

                ProjectReflectionShortcuts.SetUpCacheEntryMethod(taskResult.GetType()).Invoke(null, new[] { _mockedCachingService, key, taskResult });

                return task;
            }

            //void method
            if (methodInfo.ReturnType == typeof(void))
            {
                return null;
            }

            //Return default values
            if (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var genericArgument = methodInfo.ReturnType.GetGenericArguments().Single();
                var defaultValue = genericArgument.GetDefaultValue();
                return CoreReflectionShortcuts.TaskFromResultMethod(genericArgument).Invoke(null, new[] { defaultValue });
            }

            return methodInfo.ReturnType.GetDefaultValue();
        }
    }
}