using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Helpers;
using IInvocation = Castle.DynamicProxy.IInvocation;
using ProjectReflectionShortcuts = LazyCache.Testing.Moq.Helpers.ReflectionShortcuts;

namespace LazyCache.Testing.Moq
{
    /// <summary>
    ///     Dynamic proxy interceptor for methods that have not been set up on a lazy cache mock.
    /// </summary>
    internal class NoSetUpInterceptor : IInterceptor
    {
        private static readonly ILogger<NoSetUpInterceptor> Logger = LoggingHelper.CreateLogger<NoSetUpInterceptor>();

        /// <summary>
        ///     Checks the last method invocation on the mock;
        ///     if Add was invoked the unexpected match is set up;
        /// </summary>
        /// <param name="invocation">The proxied method invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            finally
            {
                if (invocation.ReturnValue != null && invocation.ReturnValue is IInvocationList mockInvocations)
                {
                    if (mockInvocations.Any() && mockInvocations.Last().Method.Name.Equals("Add"))
                    {
                        Logger.LogDebug("I have detected that the previous mock invocation was an add");

                        var lastInvocation = mockInvocations.Last();
                        var methodInfo = lastInvocation.Method;
                        var args = lastInvocation.Arguments;

                        //We have everything we need to set up a match, so let's do it
                        var key = args[0].ToString();
                        var value = args[1];

                        ProjectReflectionShortcuts.SetUpCacheEntryMethod(value.GetType()).Invoke(null, new[] { ((Mock<IAppCache>) invocation.Proxy).Object, key, value });
                    }
                }
            }
        }
    }
}