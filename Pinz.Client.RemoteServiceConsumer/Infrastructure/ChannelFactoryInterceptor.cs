using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl;
using Common.Logging;
using Ninject;
using Ninject.Extensions.Interception;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Com.Pinz.Client.RemoteServiceConsumer.Infrastructure
{
    public class ChannelFactoryInterceptor : IInterceptor
    {
        private static readonly ILog Log = LogManager.GetLogger<ChannelFactoryInterceptor>();
        private static readonly MethodInfo handleAsyncMethodInfo =
            typeof(ChannelFactoryInterceptor).GetMethod("HandleAsyncWithResult", BindingFlags.Instance | BindingFlags.NonPublic);


        private IServiceRunningIndicator indicator;
        private System.Object lockThis = new System.Object();

        [Inject]
        public ChannelFactoryInterceptor(IServiceRunningIndicator indicator)
        {
            this.indicator = indicator;
        }

        public void Intercept(IInvocation invocation)
        {
            lock (lockThis)
            {
                LogBefore(invocation);
                indicator.IsServiceRunning = true;
                ServiceBase serviceBase = invocation.Request.Target as ServiceBase;
                serviceBase.OpenChannel();

                try
                {
                    var delegateType = GetDelegateType(invocation);
                    if (delegateType == MethodType.Synchronous)
                    {
                        invocation.Proceed();
                        LogAfter(invocation);
                        serviceBase.CloseChannel();
                        indicator.IsServiceRunning = false;
                    }
                    if (delegateType == MethodType.AsyncAction)
                    {
                        invocation.Proceed();
                        invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue, invocation);
                    }
                    if (delegateType == MethodType.AsyncFunction)
                    {
                        invocation.Proceed();
                        ExecuteHandleAsyncWithResultUsingReflection(invocation);
                    }
                    LogAfter(invocation);
                }
                catch (FaultException ex)
                {
                    serviceBase.CloseChannel();
                    indicator.IsServiceRunning = false;
                    if (ex.InnerException != null)
                    {
                        Log.Fatal("Falied to execute call ! ", ex.InnerException);
                        throw ex.InnerException;
                    }
                    Log.Fatal("Falied to execute call ! ", ex);
                    throw;
                }
                catch (Exception ex2)
                {
                    serviceBase.CloseChannel();
                    indicator.IsServiceRunning = false;
                    Log.Fatal("Falied to execute call ! ", ex2);
                    throw;
                }
            }
        }

        private void LogAfter(IInvocation invocation)
        {
            var methodName = invocation.Request.Method.Name;
            if (invocation.Request.Method.ReturnType != typeof(void))
            {
                Log.DebugFormat("Method {0} returned <{1}>", methodName, invocation.ReturnValue);
            }
        }

        private void LogBefore(IInvocation invocation)
        {
            var methodName = invocation.Request.Method.Name;

            var parameterNames = invocation.Request.Method.GetParameters().Select(p => p.Name).ToList();
            var parameterValues = invocation.Request.Arguments;

            var message = string.Format("Method {0} called with parameters ", methodName);
            for (int index = 0; index < parameterNames.Count; index++)
            {
                var name = parameterNames[index];
                var value = parameterValues[index];
                message += string.Format("<{0}>:<{1}>,", name, value);
            }

            //log method called
            Log.Debug(message);
        }

        private void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
        {
            var resultType = invocation.Request.Method.ReturnType.GetGenericArguments()[0];
            var mi = handleAsyncMethodInfo.MakeGenericMethod(resultType);
            invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue, invocation });
        }

        private async Task HandleAsync(Task task, IInvocation invocation)
        {
            ServiceBase serviceBase = invocation.Request.Target as ServiceBase;
            try
            {
                await task;
                LogAfter(invocation);
                serviceBase.CloseChannel();
                indicator.IsServiceRunning = false;
            }
            catch (FaultException ex)
            {
                serviceBase.CloseChannel();
                indicator.IsServiceRunning = false;
                if (ex.InnerException != null)
                {
                    Log.Fatal("Falied to execute call ! ", ex.InnerException);
                    throw ex.InnerException;
                }
                Log.Fatal("Falied to execute call ! ", ex);
                throw;
            }
            catch (Exception ex2)
            {
                serviceBase.CloseChannel();
                indicator.IsServiceRunning = false;
                Log.Fatal("Falied to execute call ! ", ex2);
                throw;
            }
        }

        private async Task<T> HandleAsyncWithResult<T>(Task<T> task, IInvocation invocation)
        {
            ServiceBase serviceBase = invocation.Request.Target as ServiceBase;
            try
            {
                T value = await task;
                LogAfter(invocation);
                serviceBase.CloseChannel();
                indicator.IsServiceRunning = false;

                return value;
            }
            catch (FaultException ex)
            {
                serviceBase.CloseChannel();
                indicator.IsServiceRunning = false;
                if (ex.InnerException != null)
                {
                    Log.Fatal("Falied to execute call ! ", ex.InnerException);
                    throw ex.InnerException;
                }
                Log.Fatal("Falied to execute call ! ", ex);
                throw;
            }
            catch (Exception ex2)
            {
                serviceBase.CloseChannel();
                indicator.IsServiceRunning = false;
                Log.Fatal("Falied to execute call ! ", ex2);
                throw;
            }
        }

        private MethodType GetDelegateType(IInvocation invocation)
        {
            var returnType = invocation.Request.Method.ReturnType;
            if (returnType == typeof(Task))
                return MethodType.AsyncAction;
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                return MethodType.AsyncFunction;
            return MethodType.Synchronous;
        }

        private enum MethodType
        {
            Synchronous,
            AsyncAction,
            AsyncFunction
        }
    }
}
