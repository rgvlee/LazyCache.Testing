using Castle.DynamicProxy;
using LazyCache.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace LazyCache.Testing.Moq {
    /// <summary>
    /// Dynamic proxy interceptor for methods that have not been set up on a lazy cache mock.
    /// </summary>
    internal class NoSetUpInterceptor : IInterceptor {
        private static readonly ILogger<NoSetUpInterceptor> Logger = LoggerHelper.CreateLogger<NoSetUpInterceptor>();

        /// <summary>
        ///     Checks the last method invocation on the mock;
        ///     if Add was invoked the unexpected match is set up;
        /// </summary>
        /// <param name="invocation">The proxied method invocation.</param>
        public void Intercept(IInvocation invocation) {
            //Logger.LogDebug($"{invocation.Method}");
            
            try {
                invocation.Proceed();
            }
            catch (Exception) {
                throw;
            }
            finally {
                if (invocation.ReturnValue != null && invocation.ReturnValue is IInvocationList mockInvocations) {
                    if (mockInvocations.Any() && mockInvocations.Last().Method.Name.StartsWith("Add", StringComparison.CurrentCultureIgnoreCase)) {
                        Logger.LogDebug("I have detected that the previous mock invocation was an add");

                        var lastInvocation = mockInvocations.Last();
                        var methodInfo = lastInvocation.Method;
                        var args = lastInvocation.Arguments;

                        //We have everything we need to set up a match, so let's do it
                        var key = args[0].ToString();
                        var value = args[1];
                        var valueType = methodInfo.GetParameters()[1].ParameterType;

                        var method = typeof(LazyCache.Testing.Moq.Extensions.MockExtensions).GetMethods().Single(mi => mi.Name.Equals("SetUpCacheEntry"));
                        method.MakeGenericMethod(valueType).Invoke(null, new object[] { invocation.Proxy, key, value });
                    };
                }

                //Logger.LogDebug(invocation.ReturnValue);
            }
        }
    }
}