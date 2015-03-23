namespace CefSharp.Internals
{
    using System.Diagnostics;
    using System.ServiceModel;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LogService : ILogService
    {
        public LogService()
        {
            var context = OperationContext.Current;
            var host = (LogServiceHost)context.Host;

            LogHandler = host.LogHandler;
        }

        public ILogHandler LogHandler { get; private set; }

        public void Log(LogMessage message)
        {
            if (LogHandler != null)
            {
                LogHandler.Log(message);
            }            
        }        
    }
}