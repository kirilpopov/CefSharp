namespace CefSharp.Internals
{
    using System;
    using System.Net.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;

    public class LogServiceHost : ServiceHost
    {
        public ILogHandler LogHandler { get; private set; }
        private const long SixteenMegaBytesInBytes = 16 * 1024 * 1024;
        
        private TaskCompletionSource<OperationContext> operationContextTaskCompletionSource = new TaskCompletionSource<OperationContext>();

        public LogServiceHost(int parentProcessId, int browserId, ILogHandler logHandler)
            : base(typeof(LogService), new Uri[0])
        {
            LogHandler = logHandler;

            var serviceName = RenderprocessClientFactory.GetLogServiceName(parentProcessId, browserId);

            Description.ApplyServiceBehavior(() => new ServiceDebugBehavior(), p => p.IncludeExceptionDetailInFaults = true);

            var binding = CreateBinding();

            var endPoint = AddServiceEndpoint(
                typeof(ILogService),
                binding,
                new Uri(serviceName)
                );

            endPoint.Contract.ProtectionLevel = ProtectionLevel.None;                        
        }

        public void SetOperationContext(OperationContext operationContext)
        {
            if (operationContextTaskCompletionSource.Task.Status == TaskStatus.RanToCompletion)
            {
                operationContextTaskCompletionSource = new TaskCompletionSource<OperationContext>();
            }

            operationContextTaskCompletionSource.SetResult(operationContext);
        }
       
        protected override void OnClose(TimeSpan timeout)
        {
            var task = operationContextTaskCompletionSource.Task;

            CloseChannel(task);

            base.OnClose(timeout);
        }

        private void CloseChannel(Task<OperationContext> task)
        {
            try
            {
                if (task.IsCompleted)
                {
                    var context = task.Result;

                    if (context.Channel != null && context.Channel.State == CommunicationState.Opened)
                    {
                        context.Channel.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
       
        protected override void OnClosed()
        {
            base.OnClosed();            
            operationContextTaskCompletionSource = null;
        }

        public static CustomBinding CreateBinding()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxReceivedMessageSize = SixteenMegaBytesInBytes;
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.SendTimeout = TimeSpan.MaxValue;
            binding.OpenTimeout = TimeSpan.MaxValue;
            binding.CloseTimeout = TimeSpan.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;

            // Ensure binding connection stays open indefinitely until closed
            var customBinding = new CustomBinding(binding);
            var connectionSettings = customBinding.Elements.Find<NamedPipeTransportBindingElement>().ConnectionPoolSettings;
            connectionSettings.IdleTimeout = TimeSpan.MaxValue;
            connectionSettings.MaxOutboundConnectionsPerEndpoint = 0;

            return customBinding;
        }
    }
}